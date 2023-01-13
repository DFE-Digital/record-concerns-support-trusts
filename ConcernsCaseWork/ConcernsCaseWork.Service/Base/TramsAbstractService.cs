using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base;

public abstract class TramsAbstractService : AbstractService
{
	protected TramsAbstractService(IHttpClientFactory clientFactory, ILogger<TramsAbstractService> logger, ICorrelationContext correlationContext, IUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
	{
		HttpClientName = "TramsClient";
	}
}