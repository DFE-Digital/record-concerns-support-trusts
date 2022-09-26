
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using Service.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Service.Redis.Nti;
using ConcernsCaseWork.Services.Nti;
using Service.TRAMS.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Decision
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{

		private readonly ILogger<AddPageModel> _logger;

		public AddPageModel(ILogger<AddPageModel> logger)
		{
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation($"{nameof(AddPageModel)}::{LoggingHelpers.EchoCallerName()}");

			try
			{
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(AddPageModel)}::{LoggingHelpers.EchoCallerName()}");

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}


		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
		
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{nameof(AddPageModel)}::{LoggingHelpers.EchoCallerName()}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
	}
}