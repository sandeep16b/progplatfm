namespace Abim.Platform.Program.App.Util
{
#pragma warning disable
    public enum TriggeringEvent
    {
        Unkonwn = 0,
        ExamResultInitial = 1,
        ExamResultMocKci = 2,
        ActivityCompleted = 3,
        ActivityCreated = 4,
        Attestation = 5,
        YearEndLookBack = 6,
        EarlyYearEndLookBack = 7,
        SeptemberFirst=8,
        IssuanceCreated=9,
        ReinstateCredential=10,
        Others = 20

    }

    public enum ExecutingProcessType
    {
        Unkonwn = 0,
        YearEndLookBack = 1,
        CorrectiveAction = 2,
        EarlyYearEndLookBack = 3,
        CoSponsoredLockOut = 4,
        Others =10
    }
#pragma warning restore 
}
