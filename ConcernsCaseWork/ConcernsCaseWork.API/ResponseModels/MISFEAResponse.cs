namespace ConcernsCaseWork.API.ResponseModels
{
    public class MISFEAResponse
    {
        public ProviderResponse Provider { get; set; }
        public string LocalAuthority { get; set; }
        public string Region { get; set; }
        public string OfstedRegion { get; set; }
        public string DateOfLatestShortInspection { get; set; }
        public string NumberOfShortInspectionsSinceLastFullInspectionRAW { get; set; }
        public string NumberOfShortInspectionsSinceLastFullInspection { get; set; }
        public string InspectionNumber { get; set; }
        public string InspectionType { get; set; }
        public string FirstDayOfInspection { get; set; }
        public string LastDayOfInspection { get; set; }
        public string DatePublished { get; set; }
        public string OverallEffectivenessRAW { get; set; }
        public string OverallEffectiveness { get; set; }
        public string QualityOfEducationRAW { get; set; }
        public string QualityOfEducation { get; set; }
        public string BehaviourAndAttitudesRAW { get; set; }
        public string BehaviourAndAttitudes { get; set; }
        public string PersonalDevelopmentRAW { get; set; }
        public string PersonalDevelopment { get; set; }
        public string EffectivenessOfLeadershipAndManagementRAW { get; set; }
        public string EffectivenessOfLeadershipAndManagement { get; set; }
        public string IsSafeguardingEffective { get; set; }
        public string PreviousInspectionNumber { get; set; }
        public string PreviousLastDayOfInspection { get; set; }
        public string PreviousOverallEffectivenessRAW { get; set; }
        public string PreviousOverallEffectiveness { get; set; }
    }
}