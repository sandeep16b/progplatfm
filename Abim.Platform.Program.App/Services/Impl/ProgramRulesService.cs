using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Hangfire;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class ProgramRulesService
    /// </summary>
    public partial class ProgramRulesService : IProgramRulesService, IDisposable
    {
        #region Properties

        /// <summary>
        /// Logger. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Product Host Url
        /// </summary>
        protected static string ProductHostUrl = System.Configuration.ConfigurationManager.AppSettings["ProductHostUrl"];

        /// <summary>
        /// Registration Host Url
        /// </summary>
        protected static string RegistrationHostUrl = System.Configuration.ConfigurationManager.AppSettings["RegistrationHostUrl"];

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token service.
        /// </value>
        protected IAccessTokenService AccessTokenService { get; set; }

        /// <summary>
        /// The Certification service
        /// </summary>
        protected ICertificationService CertificationService { get; set; }

        /// <summary>
        /// The Credential service
        /// </summary>
        protected ICredentialService CredentialService { get; set; }

        /// <summary>
        /// The Source service
        /// </summary>
        protected ISourceService SourceService { get; set; }

        /// <summary>
        /// Product Interservice
        /// </summary>
        protected IProductInterservice ProductInterservice { get; set; }

        /// <summary>
        /// Registration Interservice
        /// </summary>
        protected IRegistrationInterservice RegistrationInterservice { get; set; }

        /// <summary>
        /// CorrectiveActionRunService
        /// </summary>
        protected ICorrectiveActionResultService CorrectiveActionRunService { get; set; } 

        /// <summary>
        /// LookBackDatesInfoService
        /// </summary>
        protected ILookBackDatesInfoService LookBackDatesInfoService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected IBusControl Bus { get; set; }

        /// <summary>
        /// Service used for logging lookback-related information
        /// </summary>
        protected ILookbackLogService LookbackLogService { get; set; }

        #endregion Properties

        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramRulesService"/> class.
        /// </summary>
        /// <param name="certificationService"></param>
        /// <param name="credentialService"></param>
        /// <param name="sourceService"></param>
        /// <param name="productInterservice"></param>
        /// <param name="registrationInterservice"></param>
        /// <param name="bus"></param>
        /// <param name="jobClient"></param>
        /// <param name="validationFactory"></param>
        /// <param name="accessTokenService"></param>
        /// <param name="correctiveActionRunService"></param>
        /// <param name="lookBackDatesInfoService"></param>
        /// <param name="lookBackLogService"></param>
        public ProgramRulesService(ICertificationService certificationService,
                                ICredentialService credentialService,
                                ISourceService sourceService,
                                IProductInterservice productInterservice,
                                IRegistrationInterservice registrationInterservice,
                                IBusControl bus,
                                IBackgroundJobClient jobClient,
                                IValidationFactory validationFactory,
                                IAccessTokenService accessTokenService,
                                ICorrectiveActionResultService correctiveActionRunService,
                                ILookBackDatesInfoService lookBackDatesInfoService, 
                                ILookbackLogService lookBackLogService)
        {
            CertificationService = certificationService;
            CredentialService = credentialService;
            SourceService = sourceService;

            ProductInterservice = productInterservice;
            RegistrationInterservice = registrationInterservice;
            AccessTokenService = accessTokenService;
            CorrectiveActionRunService = correctiveActionRunService;
            LookBackDatesInfoService = lookBackDatesInfoService;
            LookbackLogService = lookBackLogService;

            Bus = bus;
        }

        #region Retrieved Properties

        /// <summary>
        /// EndOf2018
        /// </summary>
        public static DateTime EndOf2018 = new DateTime(2018, 12, 31);

        /// <summary>
        /// EndOf2018
        /// </summary>
        public static DateTime DateOf2014 = new DateTime(2014, 01, 01);

        /// <summary>
        /// Local property to keep First Issuance Date for user. 
        /// </summary>
        private DateTime? _firstIssuanceDate;

        /// <summary>
        /// First ABIM issuance for member
        /// </summary>
        private DateTime FirstIssuanceDate
        {
            get
            {
                if (!_firstIssuanceDate.HasValue)
                    _firstIssuanceDate = CredentialService.GetFirstIssuanceDate(MemberId);

                if (!_firstIssuanceDate.HasValue)
                {
                    Log.Warn($"No issuances found for MemberId: '{MemberId}'");
                    throw new ApplicationException(Constants.StopFurtherExecutionMessage);
                }

                return _firstIssuanceDate.Value;
            }
        }

        private List<ActivityResource> _userActivities;
        /// <summary>
        /// User's activities
        /// </summary>
        private List<ActivityResource> UserActivities
        {
            get
            {
                if (_userActivities == null)
                    _userActivities = GetUser10yearsActivities().Result;
                return _userActivities;
            }
        }

        private UserRegistrationsAndCMPRegistrationsResource _allRegistrations;

        private IList<LongitudinalEnrollmentSummaryResource> _longitudinalEnrollments;

        /// <summary>
        /// Registration records for the user
        /// </summary>
        private IList<RegistrationResource> Registrations
        {
            get
            {
                if (_allRegistrations == null)
                    _allRegistrations = GetAllRegistrationsAndCMPRegistrationsForUser().Result;

                return _allRegistrations.Registrations;
            }
        }

        private IList<CMPRegistrationResource> CMPRegistrations
        {
            get
            {
                if (_allRegistrations == null)
                    _allRegistrations = GetAllRegistrationsAndCMPRegistrationsForUser().Result;

                return _allRegistrations.CMPRegistrations;
            }
        }

        private IList<LongitudinalEnrollmentSummaryResource> LongitudinalEnrollments
        {
            get
            {
                if (_longitudinalEnrollments == null)
                    _longitudinalEnrollments = GetLongitudinalEnrollments().Result.Data;

                return _longitudinalEnrollments;
            }
        }

        private string _accessToken;

        /// <summary>
        /// Credential records for the user 
        /// </summary>
        private IEnumerable<Credential> Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = CredentialService.SearchByMemberId(MemberId);
                return _credentials;
            }
        }

        private IEnumerable<Credential> _credentials;

        /// <summary>
        /// AccessToke for interservices comuncation
        /// </summary>
        private string AccessToken
        {
            get
            {
                if (_accessToken == null)
                    _accessToken = AccessTokenService.GetAccessToken();
                return _accessToken;
            }
        }

        private Guid _fPHMCertificateGuid;

        /// <summary>
        /// Get FPHM Certification Guid
        /// </summary>
        private Guid FPHMCertificateGuid
        {
            get
            {
                if (Guid.Empty == _fPHMCertificateGuid)
                    _fPHMCertificateGuid = CertificationService.GetByCode(ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine).ExternalId;
                return _fPHMCertificateGuid;
            }
        }

        /// <summary>
        /// Local property to keep member id
        /// </summary>
        private Guid MemberId { get; set; }

        /// <summary>
        /// Marking public to be able to set from unit tests
        /// </summary>
        public DateTime ProcessingDate { get; set; }

        /// <summary>
        /// EventDate
        /// </summary>
        private DateTime EventDate { get; set; }

        /// <summary>
        /// ExecutingProcess
        /// </summary>
        public ExecutingProcessType ExecutingProcess { get; set; } = ExecutingProcessType.Unkonwn;

        #endregion

        #region Disposal

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CertificationService.Dispose();
            CredentialService.Dispose();
            SourceService.Dispose();

            CorrectiveActionRunService.Dispose();
        }

        #endregion

    }
}
