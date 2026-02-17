using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// IMemberValuesService interface.
    /// </summary>
    public interface ILookBackDatesInfoService : IService<LookBackDatesInfo>
    {
        /// <summary>
        /// Handle(UpdateLookBackDatesInfoCommand command)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task Handle(UpdateLookBackDatesInfoCommand command);

        /// <summary>
        /// GetLookBackDatesInfo
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<LookBackDatesInfo> GetLookBackDatesInfo(Guid memberId);

        /// <summary>
        /// GetExpiredLookBackDatesInfo
        /// </summary>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        Task<IEnumerable<LookBackDatesInfo>> GetExpiredLookBackDatesInfo(DateTime expiredDate);
    }
}
