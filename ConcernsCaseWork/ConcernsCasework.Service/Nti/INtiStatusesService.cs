namespace ConcernsCasework.Service.Nti
{
	public interface INtiStatusesService
	{
		public Task<ICollection<NtiStatusDto>> GetNtiStatusesAsync();
	}
}
