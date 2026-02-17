using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// ILookbackDateLogService
    /// </summary>
    public interface ILookbackDateLogService: 
        IService<LookbackDateLog>,
        ICommandHandler<AddLookbackDateLogEntry>,
        ICommandValidationHandler<AddLookbackDateLogEntry>
    {


    }
}
