using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;
using System.Linq;

namespace Abim.Platform.Program.Tests.Setup.DomainBuilders
{
    public static class CertificationBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static Certification Build()
        {
            return Build(SourceBuilder.Build());
        }

        public static Certification Build(Source source)
        {
            return Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build());
        }

        public static Certification Build(Source source, string code)
        {
            return Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), code, RandomString.Build());
        }

        public static Certification BuildWithoutRandoms(Source source, string code, CertificationType certType, string name)
        {
            return Certification.Create(
                null, 
                source, 
                certType, 
                name, 
                code,  
                "Unit Test");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="certType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Certification Build(string code="IM",
                                          string name= null,
                                          CertificationType certType = CertificationType.Subspecialty,
                                          Source source = null)
        {
            string[] SubspecialtyArray = { "ACHD", "ADOL", "AHFTC", "CARD", "CCEP", "CRIT", "ENDO", "GAST", "GERI", "HEMA", "HPM", "ICARD", "ID", "NEPH", "ONCO", "PTHEP", "PULM", "RHEUM", "SLEEP", "SPORT", "THEP", "CLI", "DLI" };
            string[] OtherBoardArray = { "ALLG", "CLI", "DLI" };
            string[] JointAgreementArray = { "ALLG" };

            source = source ?? (!OtherBoardArray.Contains(code) ? SourceBuilder.BuildAbim() : SourceBuilder.BuildOtherBoard());

            if (code ==  ProgramResourceConstants.CertificationCode.InternalMedicine)
            {
                name = "Internal Medicine";
                certType = CertificationType.Primary;
            }
            else if (code ==  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine)
            {
                name = "Focused Practice in Hospital Medicine";
                certType = CertificationType.FocusPractice;
            }
            else if (SubspecialtyArray.Contains(code))
            {
                name = name ?? "Name_" + code;
                certType = CertificationType.Subspecialty;
            }
            else if (JointAgreementArray.Contains(code))
            {
                name = name ?? "Name_" + code;
                certType = CertificationType.JointAgreement;
            }
            else
            {
                name = name ?? "Name_" + code;
                certType = CertificationType.OtherBoard;
            }

            return Certification.Create(
                null,
                source,
                certType,
                name,
                code,
                "UnitTest" + code);
        }

    }
}
