using System.Security.Claims;

namespace ConcernsCaseWork.UserContext;

public interface IClientUserInfoService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);

	bool AddUserInfoRequestHeaders(HttpClient client);
	UserInfo UserInfo { get; }
}