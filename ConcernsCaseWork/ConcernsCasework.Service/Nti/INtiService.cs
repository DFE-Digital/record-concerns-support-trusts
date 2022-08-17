namespace ConcernsCasework.Service.Nti
{
	public interface INtiService
	{
		public Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn);
	}
}
