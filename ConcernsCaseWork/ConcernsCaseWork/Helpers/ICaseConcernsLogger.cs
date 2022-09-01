using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.Helpers
{
	public interface ICaseConcernsLogger<T> : ILogger<T>
	{
	}
}