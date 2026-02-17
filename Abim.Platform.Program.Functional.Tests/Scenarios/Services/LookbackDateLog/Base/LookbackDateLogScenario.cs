using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentValidation;
using MassTransit;
using NHibernate;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.LookbackDateLog.Base
{
    /// <summary>
    /// LookbackDateLogScenario
    /// </summary>
    public abstract class LookbackDateLogScenario : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ILookbackDateLogRepository));
            types.Add(typeof(ISession));
            types.Add(typeof(IBusControl));
            types.Add(typeof(IQueryFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidator<AddLookbackDateLogEntry>));
            return types;
        }

    }
}
