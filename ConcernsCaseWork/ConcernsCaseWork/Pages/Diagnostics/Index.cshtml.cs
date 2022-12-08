using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Globalization;

using System;

namespace ConcernsCaseWork.Pages.Diagnostics
{
	[Authorize]

    public class IndexModel : PageModel
    {
	    private readonly IConfiguration _configuration;
	    private readonly IWebHostEnvironment _env;
        public string Env { get; set; }
	    public string ReleaseTag { get; set; }


		public IndexModel(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}

        public void OnGet()
        {
	        this.ReleaseTag = _configuration["ConcernsCasework:ReleaseTag"];

	        if (_env.IsDevelopment() || _env.IsStaging())
	        {
		        this.Env = _env.IsDevelopment() ? "Development" : "Staging";
	        }
        }

    }
}
