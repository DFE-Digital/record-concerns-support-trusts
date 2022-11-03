namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public interface INtiWarningLetterReasonsService
	{
		Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync();
	}
}
