namespace ConcernsCasework.Service.Base
{
	public class TramsAbstractService
	{
		internal readonly IHttpClientFactory ClientFactory;
		internal const string HttpClientName = "TramsClient";
		internal const string EndpointsVersion = "v2";
		internal const string EndpointPrefix = "concerns-cases";
		
		protected TramsAbstractService(IHttpClientFactory clientFactory)
		{
			ClientFactory = clientFactory;
		}
	}
}