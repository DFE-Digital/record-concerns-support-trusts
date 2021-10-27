using System.Net.Http;

namespace Service.TRAMS.Base
{
	public class AbstractService
	{
		internal readonly IHttpClientFactory ClientFactory;
		internal const string HttpClientName = "TramsClient";
		internal const string EndpointsVersion = "v2";
		internal const string EndpointPrefix = "concerns-cases";
		
		protected AbstractService(IHttpClientFactory clientFactory)
		{
			ClientFactory = clientFactory;
		}
	}
}