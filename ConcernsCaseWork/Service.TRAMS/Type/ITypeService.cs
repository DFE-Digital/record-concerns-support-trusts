using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface ITypeService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}