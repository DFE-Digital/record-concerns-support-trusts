using System.Security.Claims;

namespace ConcernsCaseWork.Service.Context;

public interface IUserContextService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);
	void AddHeaders(HttpRequestMessage request);
}