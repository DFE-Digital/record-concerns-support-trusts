using ConcernsCaseWork.Attributes;
using ConcernsCaseWork.UserContext;
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
		private readonly IClientUserInfoService _userInfoService;
		public string Env { get; set; }
		public string ReleaseTag { get; set; }

		public string ModuleVersionId { get; set; }
		public Version AssemblyVersion { get; set; }
		public string AssemblyFileVersion { get; set; }
		public string BuildGuid { get; set; }
		public string BuildTime { get; set; }
		public string BuildMode { get; set; }
		public string BuildMessage { get; set; }

		public IndexModel(IConfiguration configuration, IWebHostEnvironment env, IClientUserInfoService userInfoService)
		{
			_configuration = configuration;
			_env = env;
			_userInfoService = userInfoService;
		}

		public void OnGet()
		{
			ReleaseTag = _configuration["ConcernsCasework:ReleaseTag"];

			if (_env.IsDevelopment() || _env.IsStaging())
			{
				Env = _env.IsDevelopment() ? "Development" : "Staging";
			}

			var assembly = Assembly.GetEntryAssembly();
			AssemblyVersion = assembly.GetName().Version;
			AssemblyFileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
			ModuleVersionId = assembly.ManifestModule.ModuleVersionId.ToString();
			BuildMode = GetBuildMode();
			BuildTime = assembly.GetCustomAttribute<BuildTimeAttribute>().BuildTime;
			BuildGuid = assembly.GetCustomAttribute<BuildGuidAttribute>().BuildGuid;
			BuildMessage = assembly.GetCustomAttribute<CustomBuildMessageAttribute>().CustomBuildMessage;

			Roles = _userInfoService.UserInfo.Roles;
		}

		public string[] Roles { get; set; }

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
