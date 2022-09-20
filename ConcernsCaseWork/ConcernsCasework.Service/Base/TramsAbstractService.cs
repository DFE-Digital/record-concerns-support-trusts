using Microsoft.Extensions.Logging;

namespace ConcernsCasework.Service.Base;

public abstract class TramsAbstractService : AbstractService
{
	protected TramsAbstractService(IHttpClientFactory clientFactory, ILogger<TramsAbstractService> logger) : base(clientFactory, logger)
	{
		HttpClientName = "TramsClient";
	}
}