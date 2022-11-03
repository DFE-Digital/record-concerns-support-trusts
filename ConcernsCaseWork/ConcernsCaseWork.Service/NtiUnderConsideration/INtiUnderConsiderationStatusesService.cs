namespace ConcernsCaseWork.Service.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesService
	{
		Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses();
	}
}
