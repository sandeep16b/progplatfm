using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Relational
{
    /// <summary>
    /// ComplexQuery Class.
    /// </summary>
    /// <remarks>
    /// TODO: Add validator(s)
    /// </remarks>
    public abstract class ComplexQueryBase
    {
        /// <summary>
        /// The page definition backing field
        /// </summary>
        private PageDefinition _pageDefinition;

        /// <summary>
        /// Gets or sets the page definition.
        /// </summary>
        /// <value>
        /// The page definition.
        /// </value>
        /// <exception cref="System.Exception">A ComplexQueryBase may not have a null PageDefinition</exception>
        public PageDefinition PageDefinition
        {
            get
            {
                return _pageDefinition;
            }
            set
            {
               if(value == null)
                    throw new Exception("A ComplexQueryBase may not have a null PageDefinition");
               _pageDefinition = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexQueryBase"/> class.
        /// </summary>
        protected ComplexQueryBase()
        {
            PageDefinition = new PageDefinition();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexQueryBase"/> class.
        /// </summary>
        /// <param name="paging">The paging.</param>
        protected ComplexQueryBase(PageDefinition paging)
        {
            PageDefinition = paging;
        }

        /// <summary>
        /// Validates the query.
        /// </summary>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public virtual bool Validate(out List<string> errorMessages)
        {
            //setup
            errorMessages = new List<string>();
            var originalErrorCount = errorMessages.Count;
            
            //validation
            List<string> pageDefinitionErrors;
            PageDefinition.Validate(out pageDefinitionErrors);
            foreach(var msg in pageDefinitionErrors)
                errorMessages.Add(msg);
            
            //return bool
            return (errorMessages.Count == originalErrorCount);
        }
    }

    /// <summary>
    /// A basic query
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.ComplexQueryBase" />
    internal class PagedQuery : ComplexQueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedQuery"/> class.
        /// </summary>
        internal PagedQuery()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedQuery"/> class.
        /// </summary>
        /// <param name="paging">The paging.</param>
        internal PagedQuery(PageDefinition paging) : base(paging)
        {
        }
    }
}
