using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace ConcernsCaseWork.UserContext;

public class ServerUserInfoService : IServerUserInfoService
{
	public Guid InstanceId { get; }
	public ServerUserInfoService()
	{
		InstanceId = Guid.NewGuid();
	}

	public UserInfo? UserInfo { get; set; }


	public void ReceiveRequestHeaders(IHeaderDictionary headers)
	{
		var simpleHeaders = headers
			.Select(X => new KeyValuePair<string, string>(X.Key, X.Value.First()))
			.ToArray();
		UserInfo = UserInfo.FromHeaders(simpleHeaders);
	}
}