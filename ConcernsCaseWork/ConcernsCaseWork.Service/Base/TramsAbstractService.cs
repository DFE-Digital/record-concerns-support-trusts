using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base;

public abstract class TramsAbstractService : AbstractService
{
	protected TramsAbstractService(IHttpClientFactory clientFactory, ILogger<TramsAbstractService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
	{
		HttpClientName = "TramsClient";
	}
}