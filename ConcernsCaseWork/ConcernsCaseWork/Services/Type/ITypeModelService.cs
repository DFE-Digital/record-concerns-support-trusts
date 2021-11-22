using ConcernsCaseWork.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Type
{
	public interface ITypeModelService
	{
		Task<TypeModel> GetTypeModel();
		Task<TypeModel> GetSelectedTypeModelByUrn(long urn);
		Task<TypeModel> GetTypeModelByUrn(long urn);
	}
}