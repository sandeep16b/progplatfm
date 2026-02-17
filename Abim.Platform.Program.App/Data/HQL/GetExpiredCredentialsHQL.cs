namespace Abim.Platform.Program.App.Data.HQL
{
    /// <summary>
    /// Static class for HQL to get expired credentials
    /// </summary>
    public static class GetExpiredCredentialsHQL
    {
        //to-do: verify that only IssuanceStatus is 'Active' should be in this SQL

        /// <summary>
        /// HQL to get expired credentials
        /// </summary>
        public const string HQL =
            @"select c.ExternalId, i.Id from Credential c join c.Issuances i
	                where i.IssuanceStatus = 'Active'
	                and (i.Duration = 'Timelimited' and i.ExpirationDate is not null and i.ExpirationDate <= :checkDate)";

        /// <summary>
        /// The member HQL
        /// </summary>
        public const string MemberHQL =
            @"select c.ExternalId, i.Id from Credential c join c.Issuances i
	                where c.MemberId = :memberId and i.IssuanceStatus = 'Active' 
	                and (i.Duration = 'Timelimited' and i.ExpirationDate is not null and i.ExpirationDate <= :checkDate)";
    }
}
