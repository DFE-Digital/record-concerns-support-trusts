using System.Security.Claims;

namespace ConcernsCaseWork.UserContext;

public interface IUserInfoService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);
	void AddHeaders(HttpRequestMessage request);
}