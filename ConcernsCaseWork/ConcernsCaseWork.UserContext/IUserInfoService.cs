using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ConcernsCaseWork.UserContext;

public interface IClientUserInfoService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);

	void AddRequestHeaders(HttpClient client);
	UserInfo? UserInfo { get; }
}

public interface IServerUserInfoService
{
	void ReceiveRequestHeaders(IHeaderDictionary headers);
	UserInfo? UserInfo { get; }
}