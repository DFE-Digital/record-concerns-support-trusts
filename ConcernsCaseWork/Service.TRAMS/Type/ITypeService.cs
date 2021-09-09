using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public interface ITypeService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}