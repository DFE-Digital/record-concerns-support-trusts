using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ConcernsCaseWork.UserContext
{
	public class UserInfo
	{
		public string Name { get; set; }
		public string[] Roles { get; set; }

		public const string CaseWorkerRoleClaim = "concerns-casework.caseworker";
		public const string TeamLeaderRoleClaim = "concerns-casework.teamleader";
		public const string AdminRoleClaim = "concerns-casework.admin";

		public const string NameHeaderKey = "x-userContext-name";
		public const string RoleHeaderKeyPrefix = "x-userContext-role-";

		public static string[] ParseRoleClaims(string[] claims)
		{
			return claims.Where(c => c.StartsWith("concerns-casework.")).ToArray();
		}

		public IEnumerable<KeyValuePair<string, string>> ToHeadersKVP()
		{
			yield return new KeyValuePair<string, string>(NameHeaderKey, this.Name);

			for (int i = 0; i < this.Roles.Length; i++)
			{
				yield return new KeyValuePair<string, string>($"{RoleHeaderKeyPrefix}{i}", this.Roles[i]);
			}
		}

		public static UserInfo FromHeaders(KeyValuePair<string,string>[] headers)
		{

			var name = headers.FirstOrDefault(x => x.Key == NameHeaderKey).Value;

			var roles = headers
				.Where(x => x.Key.StartsWith(RoleHeaderKeyPrefix) && x.Value.StartsWith("concerns-casework."))
				.Select(x => x.Value)
				.ToArray();

			return new UserInfo() { Name = name, Roles = roles };
		}


		public bool IsCaseworker()
		{
			return this.Roles.Contains(CaseWorkerRoleClaim);
		}

		public bool IsTeamLeader()
		{
			return this.Roles.Contains(TeamLeaderRoleClaim);
		}

		public bool IsAdmin()
		{
			return this.Roles.Contains(AdminRoleClaim);
		}
	}
}
