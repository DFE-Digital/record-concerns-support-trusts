namespace Concerns.Data.RequestModels
{
    public class TransferringAcademiesRequest
    {
        public string OutgoingAcademyUkprn { get; set; }
        public string IncomingTrustUkprn { get; set; }
        public string PupilNumbersAdditionalInformation { get; set; }
        public string LatestOfstedReportAdditionalInformation { get; set; }
        public string KeyStage2PerformanceAdditionalInformation { get; set; }
        public string KeyStage4PerformanceAdditionalInformation { get; set; }
        public string KeyStage5PerformanceAdditionalInformation { get; set; }
    }
}