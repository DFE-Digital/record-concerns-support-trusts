using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Type
{
	public interface ITypeModelService
	{
		Task<IDictionary<string, IList<string>>> GetTypes();
	}
}