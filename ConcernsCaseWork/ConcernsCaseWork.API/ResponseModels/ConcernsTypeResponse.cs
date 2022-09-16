namespace ConcernsCaseWork.API.ResponseModels
{
    public class ConcernsTypeResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Urn { get; set; }
    }
}