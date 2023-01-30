using Microsoft.AspNetCore.Http;

namespace ConcernsCaseWork.UserContext;

public interface IServerUserInfoService
{
	void ReceiveRequestHeaders(IHeaderDictionary headers);
	UserInfo? UserInfo { get; }
}