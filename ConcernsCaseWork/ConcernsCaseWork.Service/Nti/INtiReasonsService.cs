namespace ConcernsCaseWork.Service.Nti
{
	public interface INtiReasonsService
	{
		public Task<ICollection<NtiReasonDto>> GetNtiReasonsAsync();
	}
}
