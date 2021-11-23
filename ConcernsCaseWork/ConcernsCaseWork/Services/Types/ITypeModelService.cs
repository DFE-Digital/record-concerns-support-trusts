using ConcernsCaseWork.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Types
{
	public interface ITypeModelService
	{
		Task<TypeModel> GetTypeModel();
		Task<TypeModel> GetSelectedTypeModelByUrn(long urn);
		Task<TypeModel> GetTypeModelByUrn(long urn);
	}
}