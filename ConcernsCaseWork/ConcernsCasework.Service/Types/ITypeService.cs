namespace ConcernsCasework.Service.Types
{
	public interface ITypeService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}