namespace Concerns.Data.ResponseModels
{
    public class ConcernsMeansOfReferralResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Urn { get; set; }
    }
}