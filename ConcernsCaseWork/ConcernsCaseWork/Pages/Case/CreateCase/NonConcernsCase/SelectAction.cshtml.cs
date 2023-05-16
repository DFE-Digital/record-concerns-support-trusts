using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases.Create;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectActionPageModel : AbstractPageModel
{
	private readonly IUserStateCachedService _cachedService;
	private readonly ILogger<SelectActionPageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private readonly ICreateCaseService _createCaseService;
	private TelemetryClient _telemetryClient;

	[BindProperty] 
	[Required(ErrorMessage = "Select an action")] 
	public Actions? SelectedAction { get; set; }
	
	public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

	public SelectActionPageModel(
		IUserStateCachedService cachedService,
		ILogger<SelectActionPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper,
		ICreateCaseService createCaseService,
		TelemetryClient telemetryClient)
	{
		_createCaseService = Guard.Against.Null(createCaseService);
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		_telemetryClient = Guard.Against.Null(telemetryClient);
	}
	
	public ActionResult OnGet()
	{
		_logger.LogMethodEntered();
		
		return Page();
	}
	
	public ActionResult OnPost()
	{
		_logger.LogMethodEntered();

		try
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			
			switch (SelectedAction)
			{
				case Actions.TFF:
					throw new NotImplementedException();

				case Actions.SRMA:
					return Redirect("/case/create/nonconcerns/srma");
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			TempData["Error.Message"] = ErrorOnPostPage;
		}
		
		return Page();
	}
	
	public enum Actions
	{
		SRMA,
		TFF
	}
}