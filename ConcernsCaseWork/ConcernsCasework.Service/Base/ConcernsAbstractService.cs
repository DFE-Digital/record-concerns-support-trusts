namespace ConcernsCasework.Service.Base;

public class ConcernsAbstractService
{
	internal readonly IHttpClientFactory ClientFactory;
	internal const string HttpClientName = "ConcernsClient";
	internal const string EndpointsVersion = "v2";
	internal const string EndpointPrefix = "concerns-cases";
		
	protected ConcernsAbstractService(IHttpClientFactory clientFactory)
	{
		ClientFactory = clientFactory;
	}
}