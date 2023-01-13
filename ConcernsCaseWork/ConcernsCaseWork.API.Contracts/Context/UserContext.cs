using Ardalis.GuardClauses;
using System.Security.Principal;

namespace ConcernsCaseWork.API.Contracts.Context
{
	public class UserContext
	{
		public string Name { get; set; }
		public string[] Roles { get; set; }

		public const string CaseWorkerRoleClaim = "concerns-casework.caseworker";
		public const string TeamLeaderRoleClaim = "concerns-casework.teamleader";
		public const string AdminRoleClaim = "concerns-casework.admin";

		public static string[] ParseRoleClaims(string[] claims)
		{
			return claims.Where(c => c.StartsWith("concerns-casework.")).ToArray();
		}

		public IEnumerable<KeyValuePair<string, string>> ToHeadersKVP()
		{
			yield return new KeyValuePair<string, string>("x-userContext-name", this.Name);

			for (int i = 0; i < this.Roles.Length; i++)
			{
				yield return new KeyValuePair<string, string>($"x-userContext-role-{i}", this.Roles[i]);
			}
		}
	}
}
