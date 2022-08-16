namespace Concerns.Data.ResponseModels
{
    public class SMARTDataResponse
    {
        public string ProbabilityOfDeclining { get; set; }
        public string ProbabilityOfStayingTheSame { get; set; }
        public string ProbabilityOfImproving { get; set; }
        public string PredictedChangeInProgress8Score { get; set; }
        public string PredictedChanceOfChangeOccurring { get; set; }
        public string TotalNumberOfRisks { get; set; }
        public string TotalRiskScore { get; set; }
        public string RiskRatingNum { get; set; }
    }
}