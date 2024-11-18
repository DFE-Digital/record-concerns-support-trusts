﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace ConcernsCaseWork.UserContext
{
	public class UserInfo
	{
		public string Name { get; set; }
		public string[] Roles { get; set; }
		public ClaimsPrincipal User { get; set;}

		private const string NameHeaderKey = "x-user-context-name";
		private const string RoleHeaderKeyPrefix = "x-user-context-role-";

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

			var name = headers.FirstOrDefault(x => x.Key.Equals(NameHeaderKey, StringComparison.InvariantCultureIgnoreCase)).Value;

			var roles = headers
				.Where(x => x.Key.StartsWith(RoleHeaderKeyPrefix, StringComparison.InvariantCultureIgnoreCase) && x.Value.StartsWith(Claims.ClaimPrefix, StringComparison.InvariantCultureIgnoreCase))
				.Select(x => x.Value)
				.ToArray();

			if (string.IsNullOrWhiteSpace(name) || roles.Length == 0)
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

		public bool IsInCaseDeleteGroup()
		{
			return this.Roles.Contains(Claims.CaseDeleteRoleClaim);
		}
	}
}
