using ConcernsCaseWork.Models;
using Service.TRAMS.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Types
{
	public interface ITypeModelService
	{
		Task<IList<TypeDto>> GetTypes();
		Task<TypeModel> GetTypeModel();
		Task<TypeModel> GetSelectedTypeModelById(long urn);
		Task<TypeModel> GetTypeModelByUrn(long urn);
	}
}