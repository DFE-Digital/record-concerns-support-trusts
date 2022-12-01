using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace ConcernsCaseWork.API.Tests.Helpers
{
	public static class HttpExtensions
	{
		public static StringContent ConvertToJson(this object toConvert)
		{
			var body = JsonConvert.SerializeObject(toConvert);

			return new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
		}
	}
}
