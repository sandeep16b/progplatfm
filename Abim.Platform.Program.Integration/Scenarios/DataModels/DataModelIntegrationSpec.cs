

using System;
using System.Collections;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Host.Config;
using FluentNHibernate.Testing;
using NHibernate;
using NUnit.Framework;
using System.Data;
 
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Integration
{
    [TestFixture]
    public class DataModelIntegrationSpec
    {
        [Test, Category("Integration")]
        public void CanCorrectlyMapSource()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                new PersistenceSpecification<Source>(_session)
                    .CheckProperty(x => x.Name, "Test exam 2 a")
                    .CheckProperty(x => x.Code, "Test ex")
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .VerifyTheMappings();
                
                trans.Rollback();
            }
        }

        [TestCase, Category("Integration")]
        public void CanCorrectlyMapCertification()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var source = SourceBuilder.Build();
                new PersistenceSpecification<Certification>(_session)
                    .CheckProperty(x => x.Name, "Test exam 2 a")
                    .CheckProperty(x => x.Code, "Test ex")
                    .CheckProperty(x => x.AddedQualification, RandomBool.IsTrue)
                    .CheckProperty(x => x.Type, EnumAttributes.RandomEntry<CertificationType>())
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .CheckReference(x => x.Source, source)
                    .VerifyTheMappings();
                    
                trans.Rollback();
            }
        }

        [Test, Category("Integration")]
        public void CanCorrectlyMapCredential()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var source = SourceBuilder.Build();
                _session.SaveOrUpdate(source);
                var certification = CertificationBuilder.Build(source);
                new PersistenceSpecification<Credential>(_session)
                    .CheckProperty(x => x.MemberId, Guid.NewGuid())
                    .CheckProperty(x => x.AssessmentMet, RandomBool.IsTrue)
                    .CheckProperty(x => x.ForcedPathway, RandomBool.IsTrue)
                    .CheckProperty(x => x.ExamFailCount, (new Random()).Next(1, 10))
                    .CheckProperty(x => x.Pathway, EnumAttributes.RandomEntry<PathwayType>())
                    .CheckProperty(x => x.IsActive, RandomBool.IsTrue)
                    .CheckProperty(x => x.Type, EnumAttributes.RandomEntry<CredentialType>())
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .CheckReference(x => x.Certification, certification)
                    .CheckProperty(x => x.SelectedToMaintain, RandomBool.IsTrue)
                    .CheckProperty(x => x.AssessmentMetDate, new DateTime(2018, 11, 28))
                    .CheckProperty(x => x.ExamDueDate, new DateTime(2018, 11, 29))
                    .CheckProperty(x => x.DisplayExamDueDate, new DateTime(2019, 11, 29))
                    .CheckProperty(x => x.MOCExamDueDate, new DateTime(2029, 12, 31))
                    .CheckProperty(x => x.KCIExamDueDate, new DateTime(2019, 12, 31))
                    .CheckProperty(x => x.GracePeriodStartDate, new DateTime(2019, 1, 1))
                    .CheckProperty(x => x.GracePeriodEndDate, new DateTime(2020, 12, 31))
                    .VerifyTheMappings();
                    
                trans.Rollback();
            }
        }

        [Test, Category("Integration")]
        public void CanCorrectlyMapLookbackLog()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                //Build needed objects
                var source = SourceBuilder.Build();
                var certification = CertificationBuilder.Build(source);
                var credential = CredentialBuilder.Build(source);
                credential.Certification = certification;
                //Save objs to database for FK refs
                _session.SaveOrUpdate(source);
                _session.SaveOrUpdate(certification);
                _session.SaveOrUpdate(credential);

                new PersistenceSpecification<LookbackLog>(_session)
                    .CheckProperty(x => x.Action, EnumAttributes.RandomEntry<LookbackActionType>())
                    .CheckProperty(x => x.Credential, credential)
                    .CheckProperty(x => x.IsPendingAction, RandomBool.IsTrue)
                    .CheckProperty(x => x.LogDate, DateTime.Now.Date)
                    .CheckProperty(x => x.Reason, EnumAttributes.RandomEntry<LookbackReasonType>())
                    .CheckProperty(x => x.Status, EnumAttributes.RandomEntry<LookbackStatusType>())
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .VerifyTheMappings();

                trans.Rollback();
            }
        }

        [Test, Category("Integration")]
        public void CanCorrectlyMapLookbackDateLog()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                new PersistenceSpecification<LookbackDateLog>(_session)
                    .CheckProperty(x => x.ChangedDate, DateTime.Now.Date)
                    .CheckProperty(x => x.MemberGuid, Guid.NewGuid())
                    .CheckProperty(x => x.LookbackDate, EnumAttributes.RandomEntry<LookbackDateType>())
                    .CheckProperty(x => x.NewValue, DateTime.Now.Date)
                    .CheckProperty(x => x.OldValue, null)
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .VerifyTheMappings();

                trans.Rollback();
            }
        }

        [Test, Category("Integration")]
        public void CanCorrectlyMapCredentialDateLog()
        {
            using (var trans = _session.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var source = SourceBuilder.Build();
                _session.SaveOrUpdate(source);

                var cert = CertificationBuilder.Build(source);
                _session.SaveOrUpdate(cert);

                var cred = CredentialBuilder.Build(source);
                cred.Certification = cert;
                _session.SaveOrUpdate(cred);

                var changedDate = DateTime.Now;
                var oldValue = changedDate.AddYears(-1);
                var newValue = changedDate.AddYears(1);

                //DateComparator used below is due to date comparison issues explained in this URL:
                //https://stackoverflow.com/questions/6992106/verifying-datetime-fluent-nhibernate-mappings
                //Someone else had already created the DateComparator class in this file, but wasn't using it.
                var dateComparator = new DateComparator();

                new PersistenceSpecification<CredentialDateLog>(_session)
                    .CheckProperty(x => x.ChangedDate, changedDate, dateComparator)
                    .CheckProperty(x => x.DateType, EnumAttributes.RandomEntry<CredentialDateType>())
                    .CheckProperty(x => x.NewValue, newValue, dateComparator)
                    .CheckProperty(x => x.OldValue, oldValue, dateComparator)
                    .CheckProperty(x => x.AuditData, AuditData.Create("Unit Test"))
                    .CheckReference(x => x.Credential, cred)
                    .VerifyTheMappings();

                trans.Rollback();
            }
        }

        [OneTimeSetUp]
        public void SetupFixture()
        {
            Startup.UseIoc();
            _session = _factory.OpenSession();
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            if(_session != null && _session.IsOpen)
                _session.Close();
        }

        [SetUp]
        public void Setup()
        {
            if(InTransaction)
                _fixtureTrans = _session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
        }

        private char RandomNonAlphaChar()
        {
            return (char)((new Random()).Next(33, 64));
        }

        public DataModelIntegrationSpec()
        {
            _factory = Startup.ConfigureOrm();
        }

        private ITransaction _fixtureTrans;
        private readonly ISessionFactory _factory;
        private ISession _session;

        private const bool InTransaction = true;
    }

    internal class DateComparator : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            var xDate = (DateTime)x;
            var yDate = (DateTime)y;

            return (xDate - yDate).Days == 0;
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
