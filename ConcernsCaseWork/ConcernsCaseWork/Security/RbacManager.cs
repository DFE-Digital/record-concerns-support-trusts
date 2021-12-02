using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public sealed class RbacManager : IRbacManager
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<RbacManager> _logger;

		public RbacManager(IConfiguration configuration, ILogger<RbacManager> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}
		
		public Task GetUserRoles(string user)
		{
			throw new System.NotImplementedException();
		}
	}
}