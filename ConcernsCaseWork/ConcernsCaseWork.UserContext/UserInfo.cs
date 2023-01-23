using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ConcernsCaseWork.UserContext
{
	public class UserInfo
	{
		public string Name { get; set; }
		public string[] Roles { get; set; }

		private const string NameHeaderKey = "x-userContext-name";
		private const string RoleHeaderKeyPrefix = "x-userContext-role-";

		public static string[] ParseRoleClaims(string[] claims)
		{
			return claims.Where(c => c.StartsWith(Claims.ClaimPrefix, StringComparison.InvariantCultureIgnoreCase)).ToArray();
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
				.Where(x => x.Key.StartsWith(RoleHeaderKeyPrefix) && x.Value.StartsWith(Claims.ClaimPrefix, StringComparison.InvariantCultureIgnoreCase))
				.Select(x => x.Value)
				.ToArray();

			if (string.IsNullOrWhiteSpace(name) || roles.Length==0)
			{
				return null;
			}
			else
			{
				return new UserInfo() { Name = name, Roles = roles };
			}
		}


		public bool IsCaseworker()
		{
			return this.Roles.Contains(Claims.CaseWorkerRoleClaim);
		}

		public bool IsTeamLeader()
		{
			return this.Roles.Contains(Claims.TeamLeaderRoleClaim);
		}

		public bool IsAdmin()
		{
			return this.Roles.Contains(Claims.AdminRoleClaim);
		}
	}
}
