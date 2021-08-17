using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Controllers
{
	public class HomeController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<HomeController> _logger;

		public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}
		
		[Authorize]
		public async Task<IActionResult> Index(int page = 1)
		{
			
			
			return View();
		}
	}
}