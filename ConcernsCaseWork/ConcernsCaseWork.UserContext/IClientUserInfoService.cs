using System.Security.Claims;

namespace ConcernsCaseWork.UserContext;

public interface IClientUserInfoService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);

	void AddRequestHeaders(HttpClient client);
	UserInfo UserInfo { get; }
}