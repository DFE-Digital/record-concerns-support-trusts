namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsService
	{
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
