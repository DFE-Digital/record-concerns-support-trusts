using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base
{
	public abstract class ConcernsAbstractService : AbstractService
	{
		protected ConcernsAbstractService(IHttpClientFactory clientFactory, ILogger<ConcernsAbstractService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			HttpClientName = "ConcernsClient";
		}
	}
}