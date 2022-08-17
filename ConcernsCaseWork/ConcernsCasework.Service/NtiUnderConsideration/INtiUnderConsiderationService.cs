namespace ConcernsCasework.Service.NtiUnderConsideration
{
	public interface INtiUnderConsiderationService
	{
		Task<NtiUnderConsiderationDto> CreateNti(NtiUnderConsiderationDto ntiDto);
		Task<NtiUnderConsiderationDto> GetNti(long ntiId);
		Task<ICollection<NtiUnderConsiderationDto>> GetNtisForCase(long caseUrn);
		Task<NtiUnderConsiderationDto> PatchNti(NtiUnderConsiderationDto ntiDto);
	}
}
