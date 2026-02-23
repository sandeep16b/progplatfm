namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// A collection of constants that will be used in this project and any that reference it.
    /// </summary>
    public static class ProgramResourceConstants
    {
        /// <summary>
        /// Basic Api Info
        /// </summary>
        public static class AppInfo
        {
            /// <summary>
            /// The assembly name
            /// </summary>
            public const string AssemblyName = "Abim.Platform.Program.Certification.Api";
            /// <summary>
            /// The assembly host name
            /// </summary>
            public const string AssemblyHostName = "Abim.Platform.Program.Certification.Api.Host";
            /// <summary>
            /// The product name
            /// </summary>
            public const string ProductName = "Abim.Platform.Program.Certification";
            /// <summary>
            /// The company
            /// </summary>
            public const string Company = "American Board of Internal Medicine";
            /// <summary>
            /// The description
            /// </summary>
            public const string Description = "Microservice to handle certifications";
            /// <summary>
            /// The copyright
            /// </summary>
            public const string Copyright = "Copyright 2016 American Board of Internal Medicine";
            /// <summary>
            /// The display name
            /// </summary>
            public const string DisplayName = "Program Configuration";
            /// <summary>
            /// The short name
            /// </summary>
            public const string ShortName = "ProgramConfiguration";

            /// <summary>
            /// The basic name
            /// </summary>
            public const string BasicName = "Program";
        }

        /// <summary>
        /// Api version information
        /// </summary>
        public static class ApiInfo
        {
            /// <summary>
            /// The version
            /// </summary>
            public const string Version = "v1.0";
        }

        /// <summary>
        /// Api Routes - Constants
        /// </summary>
        public static class Routes
        {
            /// <summary>
            /// Prefixes
            /// </summary>   
            public static class Prefix
            {
                //parent prefix                
                /// <summary>
                /// The application
                /// </summary>
                public const string App = "api/" + ApiInfo.Version;

                //individual controller prefixes       
                               
                /// <summary>
                /// The certification
                /// </summary>
                public const string Certification = App /*+ "/certifications"*/;

                /// <summary>
                /// The credential
                /// </summary>
                public const string Credential = App /*+ "/credentials"*/;

                /// <summary>
                /// The source
                /// </summary>
                public const string Source = App /*+ "/sources"*/;

                /// <summary>
                /// The enum
                /// </summary>
                public const string Enum = App /*+ "/certifications/enums"*/;

                /// <summary>
                /// The program rules
                /// </summary>
                public const string ProgramRules = App /*+ "/programRules"*/;

                /// <summary>
                /// The PhysicianCertification for Public Site VOC Page
                /// </summary>
                public const string PhysicianCertification = App /*+ "/programRules"*/;
            }                                         
                                                      
            /// <summary>                             
            /// Anonymous System Routes               
            /// </summary>                            
            public static class App                          
            {
                /// <summary>
                /// The version
                /// </summary>
                public const string Version = "app/version";

                /// <summary>
                /// The uptime
                /// </summary>
                public const string Uptime = "app/uptime";

                /// <summary>
                /// The server time
                /// </summary>
                public const string Servertime = "app/servertime";


                /// <summary>
                /// The status
                /// </summary>
                public const string Status = "app/status";
            }                                         
                                                      
            /// <summary>                             
            /// System Routes                         
            /// </summary>                            
            public static class System                        
            {
                /// <summary>
                /// The bus information
                /// </summary>
                public const string BusInfo = "system/bus";

                /// <summary>
                /// The bus information ext
                /// </summary>
                public const string BusInfoExt = "system/bus.{ext}";

                /// <summary>
                /// The version information
                /// </summary>
                public const string VersionInfo = "system/version";

                /// <summary>
                /// The version information ext
                /// </summary>
                public const string VersionInfoExt = "system/version.{ext}";

                /// <summary>
                /// The system information
                /// </summary>
                public const string SystemInfo = "system";

                /// <summary>
                /// The system information ext
                /// </summary>
                public const string SystemInfoExt = "system.{ext}";

                /// <summary>
                /// The system root
                /// </summary>
                public const string SystemRoot = "system/root";

                /// <summary>
                /// The system root ext
                /// </summary>
                public const string SystemRootExt = "system/root.{ext}";
            }                                                      
                                                                   
            /// <summary>                                          
            /// Certification Route templates                      
            /// </summary>                                         
            public static class Certifications                            
            {
                /// <summary>
                /// The certification options
                /// </summary>
                public const string CertificationOptions = "certifications";

                /// <summary>
                /// The certification enum values
                /// </summary>
                public const string CertificationEnumValues = "certification/enum/{name}";

                /// <summary>
                /// The get certifications
                /// </summary>
                public const string GetCertifications = "certifications";

                /// <summary>
                /// The get certification by identifier
                /// </summary>
                public const string GetCertificationById = "certification/{id:Guid}";

                /// <summary>
                /// The get certification invalid identifier
                /// </summary>
                public const string GetCertificationInvalidId = "certification/{invalidId}";

                /// <summary>
                /// The get current user certifications
                /// </summary>
                public const string GetCurrentUserCertifications = "myCertifications";

                /// <summary>
                /// The get current user certifications post
                /// </summary>
                public const string GetCurrentUserCertificationsPost = "myCertifications";

                /// <summary>
                /// The get certifications by member identifier
                /// </summary>
                public const string GetCertificationsByMemberId = "certifications/member/{memberId:Guid}";

                /// <summary>
                /// The get certifications by member identifier post
                /// </summary>
                public const string GetCertificationsByMemberIdPost = "certifications/member/{memberId:Guid}";

                /// <summary>
                /// The add certification (post)
                /// </summary>
                public const string AddCertification = "certification";

                /// <summary>
                /// The update certification (put)
                /// </summary>
                public const string UpdateCertification = "certification";
            }                                                      
                                                                   
            /// <summary>                                          
            /// Credential Route templates                         
            /// </summary>                                         
            public static class Credentials                               
            {
                /// <summary>
                /// The credential options
                /// </summary>
                public const string CredentialOptions = "credentials";

                /// <summary>
                /// The credential issuance options
                /// </summary>
                public const string CredentialIssuanceOptions = "credentials/issuances";

                /// <summary>
                /// The credential enum values
                /// </summary>
                public const string CredentialEnumValues = "credential/enum/{name}";

                /// <summary>
                /// The issuance enum values
                /// </summary>
                public const string IssuanceEnumValues = "credential/issuance/enum/{name}";

                /// <summary>
                /// The get credentials
                /// </summary>
                public const string GetCredentials = "credentials";

                /// <summary>
                /// The get credentials by member identifier
                /// </summary>
                public const string GetCredentialsByMemberId = "credentials/member/{memberId:Guid}";

                /// <summary>
                /// The get credentials route ext
                /// </summary>
                public const string GetCredentialsRouteExt = "credentials.{ext}";

                /// <summary>
                /// The get current user credentials
                /// </summary>
                public const string GetCurrentUserCredentials = "myCredentials";

                /// <summary>
                /// The get current user credentials post
                /// </summary>
                public const string GetCurrentUserCredentialsPost = "myCredentials";

                /// <summary>
                /// The get credential by identifier
                /// </summary>
                public const string GetCredentialById = "credential/{id:Guid}";

                /// <summary>
                /// The get credential invalid identifier
                /// </summary>
                public const string GetCredentialInvalidId = "credential/{invalidId}";

                /// <summary>
                /// The get first abim issuance date
                /// </summary>
                public const string GetFirstABIMIssuanceDate = "firstIssuanceDate/{memberId:Guid}";

                /// <summary>
                /// The get latest lookback date
                /// </summary>
                public const string GetLatestLookbackDate = "latestLookbackDate/{memberId:Guid}";

                /// <summary>
                /// The update selected to maintain
                /// </summary>
                public const string UpdateSelectedToMaintain = "credential/{credentialId:Guid}/selectedToMaintain";

                /// <summary>
                /// The update selected to maintain invalid identifier
                /// </summary>
                public const string UpdateSelectedToMaintainInvalidId = "credential/{invalidId}/selectedToMaintain";

                /// <summary>
                /// Update pathway
                /// </summary>
                public const string UpdatePathway = "credential/{credentialId:Guid}/pathway";

                /// <summary>
                /// Update pathway invalid identifier
                /// </summary>
                public const string UpdatePathwayInvalidId = "credential/{invalidId}/pathway";

                /// <summary>
                /// Withdraw credential by setting most recent issuance status to Suspend/Revoke/Surrender and set WithDrawnDate
                /// </summary>
                public const string WithdrawCredential = "credential/{credentialId:Guid}/withdrawCredential";

                /// <summary>
                /// The update (WithdrawCredential) credential invalid identifier
                /// </summary>
                public const string WithdrawCredentialInvalidId = "credential/{invalidId}/withdrawCredential";

                /// <summary>
                /// Reinstate Credential from Suspend/Revoke/Surrender status
                /// </summary>
                public const string ReinstateCredential = "credential/{credentialId:Guid}/reinstateCredential";

                /// <summary>
                /// The update ( ReinstateCredential ) credential invalid identifier
                /// </summary>
                public const string ReinstateCredentialInvalidId = "credential/{invalidId}/reinstateCredential";

                /// <summary>
                /// Deselect Credentials
                /// </summary>
                public const string MarkForSelectionOrDeselection = "credentials/markforselectionordeselection";

                /// <summary>
                /// The add Credential (post)
                /// </summary>
                public const string AddCredential = "credential";

                /// <summary>
                /// The update Credential (put)
                /// </summary>
                public const string UpdateCredential = "credential";

                /// <summary>
                /// The add issuance (post)
                /// </summary>
                public const string AddIssuance = "issuance";

                /// <summary>
                /// The update issuance (put)
                /// </summary>
                public const string UpdateIssuance = "issuance";

                /// <summary>
                /// The Get Non Abim Issuance Count
                /// </summary>
                public const string GetNonAbimIssuancesCount = "getNonAbimIssuancesCount";

                /// <summary>
                /// Enroll In CMP
                /// </summary>
                public const string EnrollInCMP = "enrollInCMP";

                /// <summary>
                /// Get VOC Letter
                /// </summary>
                public const string GetVocLetter = "vocletter/{abimId}";



                /// <summary>
                /// UnEnroll In CMP
                /// </summary>
                public const string UnEnrollInCMP = "unenrollInCMP";
            }

            /// <summary>                                          
            /// Source Route templates                             
            /// </summary>                                             
            public static class Sources                                   
            {
                /// <summary>
                /// The source options
                /// </summary>
                public const string SourceOptions = "sources";

                /// <summary>
                /// The source enum values
                /// </summary>
                public const string SourceEnumValues = "source/enum/{name}";

                /// <summary>
                /// The get sources
                /// </summary>
                public const string GetSources = "sources";

                /// <summary>
                /// The get source by identifier
                /// </summary>
                public const string GetSourceById = "source/{id:Guid}";

                /// <summary>
                /// The get source invalid identifier
                /// </summary>
                public const string GetSourceInvalidId = "source/{invalidId}";
            }

            /// <summary>
            /// PhysicianCredentials
            /// </summary>
            public static class PhysicianCredentials
            {
                /// <summary>
                /// GetCredentialsByNPI
                /// </summary>
                public const string GetPhysicianCredentialsByNPI = "physicianCredentials/npi/{npi}";

                /// <summary>
                /// GetProfilesByNameAndDob
                /// </summary>
                public const string SearchProfilesByNameAndDob = "searchProfilesByNameAndDob";

                /// <summary>
                /// GetCredentialsByAbimId
                /// </summary>
                public const string GetPhysicianCredentialsByAbimId = "physicianCredentials/abimid/{abimid}";
            }
        }       
                                                                 
        /// <summary>                                              
        /// Api Route Names - Constants                            
        /// </summary>                                             
        public static class RouteNames                             
        {                                                          
            /// <summary>                                          
            /// Anonymous System Route Names                       
            /// </summary>                                                   
            public class App                                       
            {
                /// <summary>
                /// The version
                /// </summary>
                public const string Version = "Get App Version";

                /// <summary>
                /// The uptime
                /// </summary>
                public const string Uptime = "Get Uptime";

                /// <summary>
                /// The uptime
                /// </summary>
                public const string Servertime = "Get Servertime";

                /// <summary>
                /// The status
                /// </summary>
                public const string Status = "Get Status";
            }                                                      
                                                                   
            /// <summary>                                          
            /// System Route Names                                 
            /// </summary>                                         
            public class System                                     
            {
                /// <summary>
                /// The bus information
                /// </summary>
                public const string BusInfo = "System Bus Info";

                /// <summary>
                /// The bus information ext
                /// </summary>
                public const string BusInfoExt = "System Bus Info (with data format extension)";

                /// <summary>
                /// The version information
                /// </summary>
                public const string VersionInfo = "System Version Info";

                /// <summary>
                /// The version information ext
                /// </summary>
                public const string VersionInfoExt = "System Version Info (with data format extension)";

                /// <summary>
                /// The system information
                /// </summary>
                public const string SystemInfo = "System Info";

                /// <summary>
                /// The system information ext
                /// </summary>
                public const string SystemInfoExt = "System Info (with data format extension)";

                /// <summary>
                /// The system root
                /// </summary>
                public const string SystemRoot = "System Root";

                /// <summary>
                /// The system root ext
                /// </summary>
                public const string SystemRootExt = "System Root (with data format extension)";
            }                                                      
                                                                   
            /// <summary>                                          
            /// Certification Route Names                          
            /// </summary>                                                  
            public class Certifications                            
            {
                /// <summary>
                /// The certification options
                /// </summary>
                public const string CertificationOptions = "Certification Options";

                /// <summary>
                /// The certification enum values
                /// </summary>
                public const string CertificationEnumValues = "Certification Enum Values";

                /// <summary>
                /// The get certifications
                /// </summary>
                public const string GetCertifications = "Get all Certifications";

                /// <summary>
                /// The get certifications post
                /// </summary>
                public const string GetCertificationsPost = "Get all Certifications (Post)";

                /// <summary>
                /// The get certification by identifier
                /// </summary>
                public const string GetCertificationById = "Get Certification by Id";

                /// <summary>
                /// The get certifications by ids
                /// </summary>
                public const string GetCertificationsByIds = "Get Certifications by Ids";

                /// <summary>
                /// The get certification invalid identifier
                /// </summary>
                public const string GetCertificationInvalidId = "Certification Invalid Id";

                /// <summary>
                /// The get current user certifications
                /// </summary>
                public const string GetCurrentUserCertifications = "Get Current User Certifications";

                /// <summary>
                /// The get current user certifications post
                /// </summary>
                public const string GetCurrentUserCertificationsPost = "Get Current User Certifications (Post)";

                /// <summary>
                /// The get certifications by member identifier
                /// </summary>
                public const string GetCertificationsByMemberId = "Get Certifications by Member Id";

                /// <summary>
                /// The get certifications by member identifier post
                /// </summary>
                public const string GetCertificationsByMemberIdPost = "Get Certifications by Member Id (Post)";

                /// <summary>
                /// The add certification post
                /// </summary>
                public const string AddCertification = "Add Certification (Post)";

                /// <summary>
                /// The update certification put
                /// </summary>
                public const string UpdateCertification = "Update Certification (Put)";

            }                                                      
                                                                   
            /// <summary>                                          
            /// Credential Route Names                             
            /// </summary>                                                      
            public class Credentials                               
            {
                /// <summary>
                /// The credential options
                /// </summary>
                public const string CredentialOptions = "Credential Options";

                /// <summary>
                /// The issuance options
                /// </summary>
                public const string IssuanceOptions = "Issuance Options";

                /// <summary>
                /// The credential enum values
                /// </summary>
                public const string CredentialEnumValues = "Credential Enum Values";

                /// <summary>
                /// The issuance enum values
                /// </summary>
                public const string IssuanceEnumValues = "Issuance Enum Values";
                
                /// <summary>
                /// The get credentials
                /// </summary>
                public const string GetCredentials = "Get all Credentials";

                /// <summary>
                /// The get credentials post
                /// </summary>
                public const string GetCredentialsPost = "Get all Credentials (Post)";

                /// <summary>
                /// The get credentials ext
                /// </summary>
                public const string GetCredentialsExt = "Get Credentials by Extension";

                /// <summary>
                /// The get current user credentials
                /// </summary>
                public const string GetCurrentUserCredentials = "Get Current User Credentials";

                /// <summary>
                /// The get current user credentials post
                /// </summary>
                public const string GetCurrentUserCredentialsPost = "Get Current User Credentials (Post)";

                /// <summary>
                /// The get credential by member identifier
                /// </summary>
                public const string GetCredentialByMemberId = "Get Credential by Member Id";

                /// <summary>
                /// The get credential by identifier
                /// </summary>
                public const string GetCredentialById = "Get Credential by Id";

                /// <summary>
                /// The get credential invalid identifier
                /// </summary>
                public const string GetCredentialInvalidId = "Credential Invalid Id";

                /// <summary>
                /// The get first abim issuance date
                /// </summary>
                public const string GetFirstABIMIssuanceDate = "Get First ABIM Issuance Date";

                /// <summary>
                /// The get latest lookback date
                /// </summary>
                public const string GetLatestLookbackDate = "Get Latest Lookback Date";

                /// <summary>
                /// The update selected to maintain
                /// </summary>
                public const string UpdateSelectedToMaintain = "Update SelectedToMaintain";

                /// <summary>
                /// The update selected to maintain invalid identifier
                /// </summary>
                public const string UpdateSelectedToMaintainInvalidId = "Update SelectedToMaintain Invalid";

                /// <summary>
                /// Update pathway
                /// </summary>
                public const string UpdatePathway = "Update Pathway";

                /// <summary>
                /// Update pathway invalid identifier
                /// </summary>
                public const string UpdatePathwayInvalidId = "Update Pathway Invalid";

                /// <summary>
                /// The withdraw Credential
                /// </summary>
                public const string WithdrawCredential = "Withdraw Credential";

                /// <summary>
                /// The withdraw Credential invalid identifier
                /// </summary>
                public const string WithdrawCredentialInvalidId = "Withdraw Credential Invalid";

                /// <summary>
                /// The Reinstate Credential
                /// </summary>
                public const string ReinstateCredential = "Reinstate Credential";

                /// <summary>
                /// The Reinstate Credential invalid identifier
                /// </summary>
                public const string ReinstateCredentialInvalidId = "Reinstate Credential Invalid";

                /// <summary>
                /// Deselect Credentials
                /// </summary>
                public const string MarkForSelectionOrDeselection = "Mark Credentials for Selection or Deselection";

                /// <summary>
                /// The Update Credential
                /// </summary>
                public const string UpdateCredential = "Update Credential";

                /// <summary>
                /// The Add Credential
                /// </summary>
                public const string AddCredential = "Add Credential";

                /// <summary>
                /// The Update Issuance
                /// </summary>
                public const string UpdateIssuance = "Update Issuance";

                /// <summary>
                /// The Add Issuance
                /// </summary>
                public const string AddIssuance = "Add Issuance";

                /// <summary>
                /// The Get Non Abim Issuance Count
                /// </summary>
                public const string GetNonAbimIssuancesCount = "Get Non-Abim Issuances Count";

                /// <summary>
                /// Enroll In CMP
                /// </summary>
                public const string EnrollInCMP = "Enroll In CMP";

                /// <summary>
                /// Get VOC Letter
                /// </summary>
                public const string GetVocLetter = "Get VOC Letter";

                /// <summary>
                /// UnEnroll In CMP
                /// </summary>
                public const string UnEnrollInCMP = "UnEnroll In CMP";
            }

            /// <summary>
            /// Source Route Names
            /// </summary>                                
            public class Sources                                 
            {
                /// <summary>
                /// The source options
                /// </summary>
                public const string SourceOptions = "Source Options";

                /// <summary>
                /// The source enum values
                /// </summary>
                public const string SourceEnumValues = "Source Enum Values";

                /// <summary>
                /// The get sources
                /// </summary>
                public const string GetSources = "Get all Sources";

                /// <summary>
                /// The get sources post
                /// </summary>
                public const string GetSourcesPost = "Get all Sources (Post)";

                /// <summary>
                /// The get source by identifier
                /// </summary>
                public const string GetSourceById = "Get Source by Id";

                /// <summary>
                /// The get source invalid identifier
                /// </summary>
                public const string GetSourceInvalidId = "Source Invalid Id";
            }

            /// <summary>
            /// PhysicianCredentials
            /// </summary>
            public class PhysicianCredentials
            {
                /// <summary>
                /// GetCredentialsForPhysicianByNPI
                /// </summary>
                public const string GetPhysicianCredentialsByNPI = "Get Profile and Credentials by NPI number";

                /// <summary>
                /// GetProfilesCredentialsForPhysicianByNameAndDOB
                /// </summary>
                public const string SearchProfilesByNameAndDob = "Search Physicians Profiles by Name and DOB";

                /// <summary>
                /// GetCredentialsForPhysicianByAbimId
                /// </summary>
                public const string GetPhysicianCredentialsByAbimId = "Get Profile and Credentials by AbimId";
            }
        }

        /// <summary>
        /// Certification Name
        /// </summary>
        public static class CertificationName
        {
            /// <summary>
            /// Internal Medicine with Focused Practice in Hospital Medicine
            /// </summary>
            public const string IMwithFPHM = "Internal Medicine with Focused Practice in Hospital Medicine";

            /// <summary>
            /// Internal Medicine
            /// </summary>
            public const string IM = "Internal Medicine";
        }

        /// <summary>
        /// Certification Code
        /// </summary>
        public static class CertificationCode
        {
            /// <summary>
            /// The internal medicine
            /// </summary>
            public const string InternalMedicine = "IM";

            /// <summary>
            /// The focused practice hospital medicine
            /// </summary>
            public const string FocusedPracticeHospitalMedicine = "HOSP";

            /// <summary>
            /// The Interventional Cardiology
            /// </summary>
            public const string InterventionalCardiology = "ICARD";

            /// <summary>
            /// The Cardiovascular Disease
            /// </summary>
            public const string CardiovascularDisease = "CARD";

            /// <summary>
            /// The Infectious Disease
            /// </summary>
            public const string InfectiousDisease = "ID";

            /// <summary>
            /// The Medical Oncology
            /// </summary>
            public const string MedicalOncology = "ONCO";

            /// <summary>
            /// The  Allergy And Immunology
            /// </summary>
            public const string AllergyAndImmunology = "ALLG";

            /// <summary>
            /// The  Clinical and Laboratory Immunology
            /// </summary>
            public const string ClinicalAndLaboratoryImmunology = "CLI";

            /// <summary>
            /// The  Geriatric Medicine
            /// </summary>
            public const string GeriatricMedicine = "GERI";

            /// <summary>
            /// The Diagnostic and Laboratory Immunology
            /// </summary>
            public const string DiagnosticAndLaboratoryImmunology = "DLI";

            /// <summary>
            /// The Pulmonary Disease
            /// </summary>
            public const string PulmonaryDisease = "PULM";

            /// <summary>
            /// The Critical Care Medicine
            /// </summary>
            public const string CriticalCareMedicine = "CRIT";

        }
    }

    /// <summary>
    /// CredentialCategoryType enums
    /// </summary>
    public enum CredentialCategoryType
    {

        /// <summary>
        /// The grand father
        /// </summary>
        GrandFather,

        /// <summary>
        /// The expired grand father
        /// </summary>
        ExpiredGrandFather,

        /// <summary>
        /// The time limited
        /// </summary>
        TimeLimited,

        /// <summary>
        /// The must be maintained
        /// </summary>
        MustBeMaintained,

        /// <summary>
        /// The initial FPHM
        /// </summary>
        InitialFPHM,

        /// <summary>
        /// The unknown
        /// </summary>
        Unknown
    }
}
