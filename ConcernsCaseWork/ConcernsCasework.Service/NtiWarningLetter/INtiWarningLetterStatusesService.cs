namespace ConcernsCasework.Service.NtiWarningLetter
{
	public interface INtiWarningLetterStatusesService
	{
		Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync();
	}
}
