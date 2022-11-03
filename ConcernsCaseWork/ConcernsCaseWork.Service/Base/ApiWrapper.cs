using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Base
{
	public sealed class ApiWrapper<T>
	{
		[JsonProperty("data")]
		public T Data { get; }
		
		[JsonConstructor]
		public ApiWrapper(T data) => (Data) = (data);
	}
}