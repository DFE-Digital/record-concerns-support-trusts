using Newtonsoft.Json;

namespace ConcernsCasework.Service.Base
{
	public sealed class ApiWrapper<T>
	{
		[JsonProperty("data")]
		public T Data { get; }
		
		[JsonConstructor]
		public ApiWrapper(T data) => (Data) = (data);
	}
}