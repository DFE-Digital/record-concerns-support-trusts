namespace ConcernsCasework.Service.Nti
{
	public interface INtiReasonsService
	{
		public Task<ICollection<NtiReasonDto>> GetNtiReasonsAsync();
	}
}
