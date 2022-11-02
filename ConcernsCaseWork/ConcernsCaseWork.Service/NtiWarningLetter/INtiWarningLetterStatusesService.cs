namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public interface INtiWarningLetterStatusesService
	{
		Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync();
	}
}
