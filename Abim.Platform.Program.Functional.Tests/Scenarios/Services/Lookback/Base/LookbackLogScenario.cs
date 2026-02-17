using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentValidation;
using Hangfire;
using MassTransit;
using NHibernate;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Lookback
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LookbackLogScenario : BaseServiceScenario
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ILookbackLogRepository));
            types.Add(typeof(ISession));
            types.Add(typeof(IQueryFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(ICredentialService));
            types.Add(typeof(IBusControl));
            types.Add(typeof(IBackgroundJobClient));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidator<AddCertificationLookbackLog>));
            return types;
        }
    }
}
