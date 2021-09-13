using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Type
{
	public interface ITypeCachedService
	{
		Task ClearData();
		Task<IList<TypeDto>> GetTypes();
		Task<TypeDto> GetTypeByNameAndDescription(string name, string description);
	}
}