using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Services.Context;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base
{
	public abstract class ConcernsAbstractService : AbstractService
	{
		protected ConcernsAbstractService(IHttpClientFactory clientFactory, ILogger<ConcernsAbstractService> logger, ICorrelationContext correlationContext, IUserContextService userContextService) : base(clientFactory, logger, correlationContext, userContextService)
		{
			HttpClientName = "ConcernsClient";
		}
	}
}