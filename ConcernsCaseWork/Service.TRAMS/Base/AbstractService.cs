using System.Net.Http;

namespace Service.TRAMS.Base
{
	public class AbstractService
	{
		internal readonly IHttpClientFactory ClientFactory;

		protected AbstractService(IHttpClientFactory clientFactory)
		{
			ClientFactory = clientFactory;
		}
	}
}