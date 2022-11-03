namespace ConcernsCaseWork.Service.Ratings
{
	public interface IRatingService
	{
		Task<IList<RatingDto>> GetRatings();
	}
}