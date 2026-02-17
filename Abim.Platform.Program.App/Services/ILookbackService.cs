using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILookbackLogService : 
        IService<LookbackLog>,
        ICommandHandler<AddCertificationLookbackLog>,
        ICommandValidationHandler<AddCertificationLookbackLog>,
        ICommandHandler<AddParticipationLookbackLog>,
        ICommandValidationHandler<AddParticipationLookbackLog>
    {
      
    }
}
