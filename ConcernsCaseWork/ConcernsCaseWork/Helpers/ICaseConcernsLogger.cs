using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Helpers
{
	public interface ICaseConcernsLogger<T> : ILogger<T>
	{
	}
}