using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using Service.Redis.Teams;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	/// <summary>
	/// NOTE: Waiting for AD integration to adjust service accordingly
	/// </summary>
	public sealed class RbacManager : IRbacManager
	{
		private readonly ILogger<RbacManager> _logger;
		private readonly ITeamsCachedService _teamsService;


		public RbacManager(ILogger<RbacManager> logger, ITeamsCachedService teamsService)
		{
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);
		}

		public async Task<IList<string>> GetSystemUsers(params string[] excludes)
		{
			// TODO: Integrate the known users from the DB with Azure graph to build up a set of users where we can identify those who aren't in the graph and may have left.

			_logger.LogMethodEntered();
			var users = await _teamsService.GetTeamOwners();
			return users.Except(excludes).ToArray();
		}
	}
}