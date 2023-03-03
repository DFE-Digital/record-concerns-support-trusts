namespace ConcernsCaseWork.Service.AzureAd;

public interface IGraphClientSettings
{
	public string AdminGroupId { get; init; }
	public string CaseWorkerGroupId { get; init; }
	public string ClientId { get; init; }
	public string ClientSecret { get; init; }
	string GraphEndpointScope { get; init; }
	public string TeamLeaderGroupId { get; init; }
	public string TenantId { get; init; }
}

// 	public GraphManager()
// 	{
// var configSettings = Settings.LoadSettings();
//
// 		var app = ConfidentialClientApplicationBuilder.Create(configSettings.ClientId)
// 			.WithClientSecret(configSettings.ClientSecret)
// 			.WithAuthority(new Uri(configSettings.Authority))
// 			.Build();
//
// 		DelegateAuthenticationProvider provider = new(async requestMessage =>
// 		{
// 			var result = await app.AcquireTokenForClient(configSettings.Scopes).ExecuteAsync();
// 			requestMessage.Headers.Authorization = new("Bearer", result.AccessToken);
// 		});
//
// 		Client = new("https://graph.microsoft.com/V1.0/", provider);
// 	}
//
// 	public static async Task<IEnumerable<User>> GetGroupMemberUserDetailsAsync(string groupName)
// 	{
// 		var memberIds = await GetGroupMemberUserIdsAsync(groupName);
// 		var userDetails = await GetUserDetailsAsync(memberIds);
//
// 		return userDetails;
// 	}
//
// 	public static async Task<IEnumerable<string>> GetGroupMemberUserIdsAsync(string groupName)
// 	{
// 		var groupId = await GetGroupIdAsync(groupName);
// 		return await GetGroupMemberIdsAsync(groupId);
// 	}
//
//
// 	private static async ValueTask<string> GetGroupIdAsync(string groupName)
// 	{
// 		var group = await Client.Groups
// 			.Request()
// 			.Filter($"startsWith(displayName,'{groupName}')")
// 			.GetAsync();
//
// 		return group.First().Id;
// 	}
//
// 	private static async ValueTask<IEnumerable<string>> GetGroupMemberIdsAsync(string groupId)
// 	{
// 		List<string> groupMemberIds = new();
// 		IGroupMembersCollectionWithReferencesPage? members;
//
// 		do
// 		{
// 			members = await Client.Groups[groupId].Members
// 				.Request()
// 				.GetAsync();
//
// 			groupMemberIds.AddRange(members.CurrentPage.Select(page => page.Id));
// 		}
// 		while (members.NextPageRequest is not null);
//
// 		return groupMemberIds;
// 	}
//
// 	private static async ValueTask<User> GetUserDetailsAsync(string memberId)
// 	{
// 		var user = await Client.Users[memberId]
// 				.Request()
// 				.Select(u => new
// 				{
// 					u.Id,
// 					u.DisplayName,
// 					u.Mail
// 				})
// 				.GetAsync();
//
// 		return user;
// 	}
//
// 	private static async ValueTask<IEnumerable<User>> GetUserDetailsAsync(IEnumerable<string> memberIds)
// 	{
// 		List<User> userDetails = new();
//
// 		foreach (var memberId in memberIds)
// 		{
// 			var user = await GetUserDetailsAsync(memberId);
// 			userDetails.Add(user);
// 		}
//
// 		return userDetails;
// 	}
// }