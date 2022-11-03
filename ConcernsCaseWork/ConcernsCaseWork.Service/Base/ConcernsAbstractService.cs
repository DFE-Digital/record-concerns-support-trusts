using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base
{
	public abstract class ConcernsAbstractService : AbstractService
	{
		protected ConcernsAbstractService(IHttpClientFactory clientFactory, ILogger<ConcernsAbstractService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			HttpClientName = "ConcernsClient";
		}
	}
}