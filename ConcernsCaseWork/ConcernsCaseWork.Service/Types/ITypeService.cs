namespace ConcernsCaseWork.Service.Types
{
	public interface ITypeService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}