namespace ConcernsCasework.Service.NtiWarningLetter
{
	public interface INtiWarningLetterConditionsService
	{
		Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync();
	}
}
