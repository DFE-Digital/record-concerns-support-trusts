using ConcernsCaseWork.API.ResponseModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Tests.Helpers
{
	public static class HttpExtensions
	{
		public static StringContent ConvertToJson(this object toConvert)
		{
			var body = JsonConvert.SerializeObject(toConvert);

			return new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
		}

		public static async Task<T> ReadResponseFromWrapper<T>(this HttpContent content) where T : class
		{
			var result = await content.ReadFromJsonAsync<ApiSingleResponseV2<T>>();

			return result.Data;
		}
	}
}
