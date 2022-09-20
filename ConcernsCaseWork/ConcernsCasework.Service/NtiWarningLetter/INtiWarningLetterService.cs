namespace ConcernsCasework.Service.NtiWarningLetter
{
	public interface INtiWarningLetterService
	{
		Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter);
		Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn);
		Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId);
		Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter);	
	}
}
