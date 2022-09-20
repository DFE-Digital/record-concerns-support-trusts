using Microsoft.Extensions.Logging;

namespace ConcernsCasework.Service.Base
{
	public class ConcernsAbstractService : AbstractService
	{
		protected ConcernsAbstractService(IHttpClientFactory clientFactory, ILogger<ConcernsAbstractService> logger) : base(clientFactory, logger)
		{
			HttpClientName = "ConcernsClient";
		}
	}
}