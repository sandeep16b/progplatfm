using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// ICorrectiveActionRunService interface.
    /// </summary>
    public interface ICorrectiveActionResultService : IService<CorrectiveActionResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctiveActionRun"></param>
        /// <returns></returns>
        Task Add(CorrectiveActionResult correctiveActionRun);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctiveActionRuns"></param>
        /// <returns></returns>
        Task Add(IEnumerable<CorrectiveActionResult> correctiveActionRuns);

    }
}
