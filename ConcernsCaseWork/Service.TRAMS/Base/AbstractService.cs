using System.Net.Http;

namespace Service.TRAMS.Base
{
	public class AbstractService
	{
		internal readonly IHttpClientFactory ClientFactory;
		internal const string HttpClientName = "TramsClient";
		
		protected AbstractService(IHttpClientFactory clientFactory)
		{
			ClientFactory = clientFactory;
		}
	}
}