namespace Concerns.Data.ResponseModels
{
    public class IFDDataResponse
    {
        public string TrustOpenDate { get; set; }
        public string LeadRSCRegion { get; set; }
        public string TrustContactPhoneNumber { get; set; }
        public string PerformanceAndRiskDateOfMeeting { get; set; }
        public string PrioritisedAreaOfReview { get; set; }
        public string CurrentSingleListGrouping { get; set; }
        public string DateOfGroupingDecision { get; set; }
        public string DateEnteredOntoSingleList { get; set; }
        public string TrustReviewWriteup { get; set; }
        public string DateOfTrustReviewMeeting { get; set; }
        public string FollowupLetterSent { get; set; }
        public string DateActionPlannedFor { get; set; }
        public string WIPSummaryGoesToMinister { get; set; }
        public string ExternalGovernanceReviewDate { get; set; }
        public string EfficiencyICFPreviewCompleted { get; set; }
        public string EfficiencyICFPreviewOther { get; set; }
        public string LinkToWorkplaceForEfficiencyICFReview { get; set; }
        public string NumberInTrust { get; set; }
        public string TrustType { get; set; }
        public AddressResponse TrustAddress { get; set; }
    }
}