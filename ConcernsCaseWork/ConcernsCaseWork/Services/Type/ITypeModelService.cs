using ConcernsCaseWork.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Type
{
	public interface ITypeModelService
	{
		Task<TypeModel> GetTypes();
	}
}