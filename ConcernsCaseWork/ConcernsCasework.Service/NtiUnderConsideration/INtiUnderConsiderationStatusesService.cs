namespace ConcernsCasework.Service.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesService
	{
		Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses();
	}
}
