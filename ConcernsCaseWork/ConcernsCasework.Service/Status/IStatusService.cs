namespace ConcernsCasework.Service.Status
{
	public interface IStatusService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}