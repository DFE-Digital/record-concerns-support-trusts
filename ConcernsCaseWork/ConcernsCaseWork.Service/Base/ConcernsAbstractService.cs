using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base
{
	public abstract class ConcernsAbstractService : AbstractService
	{
		protected ConcernsAbstractService(IHttpClientFactory clientFactory, ILogger<ConcernsAbstractService> logger) : base(clientFactory, logger)
		{
			HttpClientName = "ConcernsClient";
		}
	}
}