using Abim.Platform.Program.Relational.Domain;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Relational.Services
{
    /// <summary>
    /// IService interface, for services
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public interface IService<out TAggregateRoot> : IDisposable
        where TAggregateRoot : IAggregateRoot
    {
        /// <summary>
        /// Check whether a Certification exists
        /// </summary>
        /// <param name="id">The Certification identifier</param>
        /// <returns></returns>
        bool Exists(Guid id);

        /// <summary>
        /// Gets a Certification
        /// </summary>
        /// <returns></returns>
        TAggregateRoot Load(Guid id);

        /// <summary>
        /// Gets Certifications
        /// </summary>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Search();

        /// <summary>
        /// Searches aggregate roots by a query
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="totalCount">The total count in the database that matches the search criteria. Out parameter</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Search(ComplexQueryBase query, out int totalCount);
        
        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="paging">The paging and sorting query.</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Search(PageDefinition paging, out int totalCount);
    }
}
