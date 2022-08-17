namespace ConcernsCasework.Service.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsService
	{
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
