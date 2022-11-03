namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public interface INtiWarningLetterConditionsService
	{
		Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync();
	}
}
