using System;
using Abim.Platform.Program.App.Domain;
using System.Collections.Generic;
using Abim.Platform.Program.Relational.Repository;

namespace Abim.Platform.Program.App.Data
{
    /// <summary>
    /// Certification Repository Interface
    /// </summary>
    public interface ICertificationRepository : IRepository<Certification, int>
    {
        /// <summary>
        /// Gets certifications which match any of the desired Id's
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        List<Certification> GetByIds(List<Guid> ids);

        /// <summary>
        /// Get certification by source (Guid) and code (string) which would uniquely find record
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Certification GetBySourceIdAndCode(Guid sourceId, string code);
    }
}
