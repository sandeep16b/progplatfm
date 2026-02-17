using Abim.Platform.Program.App.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abim.Platform.Program.Relational.Repository;

namespace Abim.Platform.Program.App.Data
{
    /// <summary>
    /// Source Repository Interface
    /// </summary>
    public interface ILookBackDatesInfoRepository : IRepository<LookBackDatesInfo, int>
    {
        /// <summary>
        /// GetExpiredYearEndMemberValues
        /// </summary>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        IEnumerable<LookBackDatesInfo> GetExpiredYearEndLookBackDatesInfo(DateTime expiredDate);

        /// <summary>
        /// UpdateLookBackDatesInfo
        /// </summary>
        /// <param name="lookBackDatesInfo"></param>
        /// <returns></returns>
        Task UpdateLookBackDatesInfo(LookBackDatesInfo lookBackDatesInfo);

        /// <summary>
        /// AddLookBackDatesInfo
        /// </summary>
        /// <param name="lookBackDatesInfo"></param>
        /// <returns></returns>
        Task AddLookBackDatesInfo(LookBackDatesInfo lookBackDatesInfo);
    }
}