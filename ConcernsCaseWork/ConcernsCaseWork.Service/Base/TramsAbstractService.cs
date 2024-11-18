using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Base;

public abstract class TramsAbstractService : AbstractService
{
	protected TramsAbstractService(IHttpClientFactory clientFactory, ILogger<TramsAbstractService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService apiTokenService) : base(clientFactory, logger, correlationContext, userInfoService, apiTokenService, "TramsClient")
	{
	}
}