using Abim.Platform.Program.Relational.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Relational
{
    /// <summary>
    /// The ABIM paging class.
    /// </summary>
    /// <remarks>
    /// Removing the Serializable attribute for now, as it seems to be causing some side effects in Post bodies.
    /// </remarks>
    //[Serializable]
    public class PageDefinition
    {
        #region Fields

        /// <summary>
        /// The default page index
        /// </summary>
        public const int DefaultPageIndex = 1;

        /// <summary>
        /// The default page size
        /// </summary>
        public const int DefaultPageSize = 50;

        /// <summary>
        /// The maximum page size
        /// </summary>
        public const int MaxPageSize = 500;

        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets or sets the index of the page (1-indexed).
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        private List<Sort> _sorts;

        /// <summary>
        /// Gets or sets the sorts.
        /// </summary>
        /// <value>
        /// The sorts.
        /// </value>
        /// <exception cref="System.Exception">A PageDefinition may not have a null Sorts list</exception>
        public List<Sort> Sorts
        {
            get
            {
                return _sorts;
            }
            set
            {
               if(value == null)
                    throw new Exception("A PageDefinition may not have a null Sorts list");
               _sorts = value;
            }
        }

        /// <summary>
        /// Gets the sorts if any, otherwise the default list of sorts (which is to sort by Id, ascending).
        /// </summary>
        /// <value>
        /// The sorts or default.
        /// </value>
        public List<Sort> SortsOrDefault()
        {
            if(_sorts.Any()) return _sorts;
            
            //default sorting behavior
            return new List<Sort>()
            {
                new Sort()
                {
                    SortBy = "Id",
                    SortDirection = SortDirection.Ascending
                }
            };
        }

        /// <summary>
        /// Makes for clearer code than having this logic everywhere.
        /// </summary>
        [JsonIgnore]
        public int SkippedItems
        {
            get
            {
                long skipped = (long)(PageIndex - 1) * PageSize;
                if(skipped <= int.MaxValue) return (int)skipped;
                return int.MaxValue;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PageDefinition"/> class.
        /// </summary>
        public PageDefinition()
        {
            PageIndex = DefaultPageIndex;
            PageSize = DefaultPageSize;
            Sorts = new List<Sort>();
        }

        #endregion

        #region Validation

        ///<summary>
        ///Error message utilized when a caller sets PageIndex to a zero
        ///</summary>
        ///<remarks>
        ///This and NegativePageIndexErrorMessage are separate error messages, in contrast with the combined ZeroOrNegativePageSizeErrorMessage
        ///for PageSize, for the special clarification on a zero PageIndex that reminds the user that PageIndex is 1-indexed
        ///</remarks>
        public const string ZeroPageIndexErrorMessage = "Zero is not a valid PageIndex. PageIndex is 1-indexed, so it must be 1 or greater.";
        
        ///<summary>
        ///Error message template utilized when a caller sets PageIndex to a negative number
        ///</summary>
        public const string NegativePageIndexErrorMessageTemplate = "Invalid PageIndex {0}. PageIndex must be positive.";

        /// <summary>
        /// Gets the negative page index error message.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        public string NegativePageIndexErrorMessage(int pageIndex)
        {
            return string.Format(NegativePageIndexErrorMessageTemplate, pageIndex);
        }

        ///<summary>
        ///Error message template utilized when a caller sets PageSize to zero or a negative number
        ///</summary>
        public const string ZeroOrNegativePageSizeErrorMessageTemplate = "Invalid PageSize {0}. PageSize cannot be zero or negative.";

        /// <summary>
        /// Gets the zero or negative page size error message.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public string ZeroOrNegativePageSizeErrorMessage(int pageSize)
        {
            return string.Format(ZeroOrNegativePageSizeErrorMessageTemplate, pageSize);
        }

        ///<summary>
        ///Error message template utilized when a caller sets PageSize beyond PageDefinition.MaxPageSize
        ///</summary>
        public const string ExcessivePageSizeErrorMessageTemplate = "Invalid PageSize {0}. PageSize cannot be exceed {1}";

        /// <summary>
        /// Gets the excessive page size error message.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public string ExcessivePageSizeErrorMessage(int pageSize)
        {
            return string.Format(ExcessivePageSizeErrorMessageTemplate, pageSize, MaxPageSize);
        }

        /// <summary>
        /// Validates the PageDefinition.
        /// </summary>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public virtual bool Validate(out List<string> errorMessages)
        {
            //Setup
            errorMessages = new List<string>();
            var originalErrorCount = errorMessages.Count;
            
            //PageIndex and PageSize validation
            if(PageIndex == 0)
                errorMessages.Add(string.Format(ZeroPageIndexErrorMessage, PageIndex));
            if(PageIndex < 0)
                errorMessages.Add(NegativePageIndexErrorMessage(PageIndex));
            if(PageSize < 1)
                errorMessages.Add(ZeroOrNegativePageSizeErrorMessage(PageSize));
            if(PageSize > MaxPageSize)
                errorMessages.Add(ExcessivePageSizeErrorMessage(PageSize));
            
            //Sort validation
            if(Sorts.Any())
            {
                //check for invalid SortBy keys
               if(Sorts.Any(sort => string.IsNullOrEmpty(sort.SortBy)))
                    errorMessages.Add("Invalid SortBy. SortBy cannot be null or empty.");
                
                //check for duplicate SortBy keys. This is an error condition because it will cause a GenericADOException
                var duplicateSortBys = Sorts.Where(s => s.SortBy != null)
                    .Select(s => s.SortBy.ToLower())
                    .Where(column => Sorts.Count(s2 => s2.SortBy.ToLower() == column)  > 1)
                    .Distinct().OrderBy(s => s).ToList();
                if(duplicateSortBys.Any())
                {
                    List<string> withSuppliedCasing = new List<string>();
                    foreach(string duplicateLower in duplicateSortBys)
                        withSuppliedCasing.Add(Sorts.Where(s => s.SortBy != null).First(s => s.SortBy.ToLower() == duplicateLower).SortBy);
                    var duplicateKeysCsv = string.Join(", ", withSuppliedCasing.ToArray());
                    errorMessages.Add(string.Format("One or more duplicate SortBy keys were given: {0}", duplicateKeysCsv));
                }
                
                //check for invalid SortDirections
                if(Sorts.Any(sort => sort.SortDirection == default(SortDirection)))
                    errorMessages.Add("Invalid SortDirection. SortDirection must be Ascending or Descending.");
            }
            
            //Return the bool
            return (errorMessages.Count == originalErrorCount);
        }
        
        #endregion
        
        /// <remarks>
        /// This is only here for the rare scenarios in which we don't have a repository method to pass this PageDefinition to. Generally, we never call this
        /// method, we simply let the RepositoryBase read our Sorts collection and tell NHibernate what to sort on
        /// </remarks>
        public IEnumerable<T> ApplySorts<T>(IEnumerable<T> collection)
        {
            return PropertySort.SortBy(collection, Sorts, false);
        }
    }

    /// <summary>
    /// Sort Class.
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public SortDirection SortDirection { get; set; }
    }

    /// <summary>
    /// SortDirection CLass.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Ascending
        /// </summary>
        Ascending = 1,

        /// <summary>
        /// Descending
        /// </summary>
        Descending = 2
    }
}
