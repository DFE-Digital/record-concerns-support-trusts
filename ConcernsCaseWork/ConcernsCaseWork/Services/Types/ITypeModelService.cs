using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Types
{
	public interface ITypeModelService
	{
		Task<IList<TypeDto>> GetTypes();
		Task<TypeModel> GetTypeModel();
		Task<TypeModel> GetSelectedTypeModelByUrn(long urn);
		Task<TypeModel> GetTypeModelByUrn(long urn);
	}
}