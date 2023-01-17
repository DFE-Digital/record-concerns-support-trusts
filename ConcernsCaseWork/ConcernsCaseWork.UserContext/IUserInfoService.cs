using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ConcernsCaseWork.UserContext;

public interface IUserInfoService
{
	void SetPrincipal(ClaimsPrincipal claimsPrincipal);

	void AddRequestHeaders(HttpClient client);
	void ReceiveRequestHeaders(IHeaderDictionary headers);

	UserInfo? UserInfo { get; }
}