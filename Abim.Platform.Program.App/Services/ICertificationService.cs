using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// ICertificationService interface
    /// </summary>
    public interface ICertificationService :
        IService<Certification>,
        //Adds
        ICommandHandler<AddPrimaryCertificationCommand>,
        ICommandValidationHandler<AddPrimaryCertificationCommand>,
        //Updates
        ICommandHandler<AddAddedQualificationCertificationCommand>,
        ICommandValidationHandler<AddAddedQualificationCertificationCommand>,
        ICommandHandler<AddSubspecialtyCertificationCommand>,
        ICommandValidationHandler<AddSubspecialtyCertificationCommand>,
        

        IAsyncCommandHandler<AddCertificationCommand, AddCertificationCommandResult>,
        ICommandValidationHandler<AddCertificationCommand>,

        IAsyncCommandHandler<UpdateCertificationCommand, UpdateCertificationCommandResult>,
        ICommandValidationHandler<UpdateCertificationCommand>
    {
        /// <summary>
        /// Gets the by code.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <returns></returns>
        Certification GetByCode(string Code);

    }
}
