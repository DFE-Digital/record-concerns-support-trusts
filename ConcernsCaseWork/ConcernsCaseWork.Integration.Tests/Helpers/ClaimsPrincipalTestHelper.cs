using ConcernsCaseWork.UserContext;
using System.Security.Claims;

namespace ConcernsCaseWork.Integration.Tests.Helpers
{
	public static class ClaimsPrincipalTestHelper
	{
		public static ClaimsPrincipal CreateCaseWorkerPrincipal(string name = null)
		{

			return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, name ?? "some.one@education.gov.uk"),
				new Claim(ClaimTypes.NameIdentifier, "1"),
				new Claim(Claims.CaseWorkerRoleClaim, Claims.CaseWorkerRoleClaim)
			}, "mock"));
		}

		public static ClaimsPrincipal CreateTeamLeaderPrincipal(string name = null)
		{

			return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, name ?? "some.one@education.gov.uk"),
				new Claim(ClaimTypes.NameIdentifier, "1"),
				new Claim(Claims.CaseWorkerRoleClaim, Claims.TeamLeaderRoleClaim)
			}, "mock"));
		}

		public static ClaimsPrincipal CreateAdminPrincipal(string name = null)
		{

			return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, name ?? "some.one@education.gov.uk"),
				new Claim(ClaimTypes.NameIdentifier, "1"),
				new Claim(Claims.CaseWorkerRoleClaim, Claims.TeamLeaderRoleClaim)
			}, "mock"));
		}
	}
}
