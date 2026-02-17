using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Standards
{
    ///<summary>
    ///Code standards check for IApplyDomainEvent implementations
    ///</summary>
    [Story(
        AsA = "programmer",
        IWant = "to be sure that I haven't forgotten any to implement IApplyDomainEvent",
        SoThat = "our code is consistent and self-describing"
        )]
    [TestFixture]
    public class IApplyDomainEventStandard
    {
        [TestCase]
        [WorkItem(89535)]
        public void RunCheck()
        {
            new IApplyDomainEventStandardSpec().BDDfy();
        }
    }

    /// <summary>
    /// The specification class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.BaseCheckSpecification" />
    public class IApplyDomainEventStandardSpec : BaseCheckSpecification
    {
        List<Type> DomainTypes;
        List<string> MissingInterfaceImplementations = new List<string>();

        public void GivenIViewTheDomainTypes()
        {
            var command = new ExpireIssuanceCommand();        //this will load the assembly
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Single(asm => asm.FullName.StartsWith("Abim.Platform.Program.App"));
            DomainTypes = assembly.SafeGetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("Abim.Platform.Program.App.Domain")
                && !t.IsInterface).ToList();
        }

        public void WhenICheckTheApplyMethodsToSeeIfEachHasAMatchingIDomainEventImplementation()
        {
            foreach(var domainType in DomainTypes)
            {
                var interfacesImplemented = domainType.GetInterfaces();
                var methods = domainType.GetMethods();
                foreach(var method in methods)
                {
                    if(method.Name == "Apply")
                    {
                        var parameters = method.GetParameters();
                        if(parameters.Length == 1)
                        {
                            var eventType = parameters[0].ParameterType;
                            if(eventType.Name.EndsWith("Event"))
                            {
                                if(!interfacesImplemented.Any(i => i.FullName.Contains(eventType.Name)))
                                {
                                    var description = string.Format("{0} (in {1})", eventType.Name, domainType.Name);
                                    MissingInterfaceImplementations.Add(description);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ThenNoneShouldBeMissing()
        {
            var fullResults = string.Join(", ", MissingInterfaceImplementations.ToArray());
            fullResults.ShouldBeEquivalentTo("");
        }
    }
}
