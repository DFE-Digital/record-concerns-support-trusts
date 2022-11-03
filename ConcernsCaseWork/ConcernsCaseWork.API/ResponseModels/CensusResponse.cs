namespace ConcernsCaseWork.API.ResponseModels
{
    public class CensusResponse
    {
        public string CensusDate { get; set; }
        public string NumberOfPupils { get; set; }
        public string NumberOfBoys { get; set; }
        public string NumberOfGirls { get; set; }
        public string PercentageSen { get; set; }
        public string PercentageFsm { get; set; }
        public string PercentageEnglishNotFirstLanguage  { get; set; }
        public string PerceantageEnglishFirstLanguage  { get; set; }
        public string PercentageFirstLanguageUnclassified  { get; set; }
        public string NumberEligableForFSM  { get; set; }
        public string NumberEligableForFSM6Years  { get; set; }
        public string PercentageEligableForFSM6Years  { get; set; }
    }
}