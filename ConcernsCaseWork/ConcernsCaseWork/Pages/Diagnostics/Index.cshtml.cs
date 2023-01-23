using ConcernsCaseWork.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace ConcernsCaseWork.Pages.Diagnostics
{
	[Authorize]

	[ResponseCache(NoStore = true, Duration = 0)]
	public class IndexModel : PageModel
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;
		public string Env { get; set; }
		public string ReleaseTag { get; set; }

		public string ModuleVersionId { get; set; }
		public Version AssemblyVersion { get; set; }
		public string AssemblyFileVersion { get; set; }
		public string AssemblyInformationalVersion { get; set; }
		public string BuildGuid { get; set; }
		public string BuildTime { get; set; }
		public string BuildMode { get; set; }
		public string BuildMessage { get; set; }

		public IndexModel(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}

		public void OnGet()
		{
			this.ReleaseTag = _configuration["ConcernsCasework:ReleaseTag"];

			var r = Request;

			switch (_env.IsDevelopment())
			{

			}

			if (_env.IsDevelopment() || _env.IsStaging())
			{
				this.Env = _env.IsDevelopment() ? "Development" : "Staging";
			}

			var assembly = Assembly.GetEntryAssembly();
			this.AssemblyVersion = assembly.GetName().Version;
			this.AssemblyFileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
			this.AssemblyInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
			this.ModuleVersionId = assembly.ManifestModule.ModuleVersionId.ToString();
			this.BuildMode = GetBuildMode();
			this.BuildTime = assembly.GetCustomAttribute<BuildTimeAttribute>().BuildTime;
			this.BuildGuid = assembly.GetCustomAttribute<BuildGuidAttribute>().BuildGuid;
			this.BuildMessage = assembly.GetCustomAttribute<CustomBuildMessageAttribute>().CustomBuildMessage;
		}



		private string GetBuildMode()
		{
#if DEBUG
			return "DEBUG";
#else
	return "RELEASE";
#endif

		}
	}
}
