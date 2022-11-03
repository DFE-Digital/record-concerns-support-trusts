namespace ConcernsCaseWork.Service.Status
{
	public interface IStatusService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}