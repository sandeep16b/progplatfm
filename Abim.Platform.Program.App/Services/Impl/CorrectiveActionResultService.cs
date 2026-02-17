using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation.Impl;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class CorrectiveActionRunService
    /// </summary>
    public class CorrectiveActionResultService : ServiceBase<CorrectiveActionResult, ICorrectiveActionResultRepository>, ICorrectiveActionResultService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Repository"></param>
        public CorrectiveActionResultService(ICorrectiveActionResultRepository Repository) :
        base(Repository)
        { }

        /// <summary>
        /// The logger is inherited from the base class. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctiveActionRun"></param>
        /// <returns></returns>
        public Task Add(CorrectiveActionResult correctiveActionRun)
        {
            Repository.BeginTransaction();

            AbimValidationResult objValidation = Repository.Add(correctiveActionRun);

            if (objValidation.Succeeded)
                Repository.CommitTransaction();
            else
            {
                Repository.RollbackTransaction();
                Log.Error($"Failed Repository.Add with message: '{objValidation.Message}'");
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctiveActionRuns"></param>
        /// <returns></returns>
        public Task Add(IEnumerable<CorrectiveActionResult> correctiveActionRuns)
        {
                Repository.BeginTransaction();
             
            AbimValidationResult objValidation = Repository.AddMany(correctiveActionRuns);

            if (objValidation.Succeeded)
                Repository.CommitTransaction();
            else
            {
                Repository.RollbackTransaction();
                Log.Error($"Failed Repository.Add with message: '{objValidation.Message}'");
            }

           return Task.FromResult<object>(null);
        }

        #region Disposal

        /// <summary>
        /// Disposes the child services.
        /// </summary>
        protected override void DisposeChildServices()
        {
        }

        #endregion

    }
}
