namespace ConcernsCaseWork.Service.Nti
{
	public interface INtiService
	{
		Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn);
		Task<NtiDto> CreateNtiAsync(NtiDto newNtiWarningLetter);
		Task<NtiDto> GetNtiAsync(long ntiWarningLetterId);
		Task<NtiDto> PatchNtiAsync(NtiDto ntiWarningLetter);
	}
}
