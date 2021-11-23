using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Types
{
	public interface ITypeService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}