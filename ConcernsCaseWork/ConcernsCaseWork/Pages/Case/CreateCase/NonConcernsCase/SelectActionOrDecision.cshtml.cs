using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SelectActionOrDecisionPageModel : AbstractPageModel
{
	private readonly IUserStateCachedService _cachedService;
	private readonly ILogger<SelectActionOrDecisionPageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private readonly ICreateCaseService _createCaseService;

	[BindProperty] 
	[Required]
	public Options? SelectedActionOrDecision { get; set; }
	
	public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);

	public SelectActionOrDecisionPageModel(
		IUserStateCachedService cachedService,
		ILogger<SelectActionOrDecisionPageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper,
		ICreateCaseService createCaseService)
	{
		_createCaseService = Guard.Against.Null(createCaseService);
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}
	
	public ActionResult OnGet()
	{
		_logger.LogMethodEntered();

		return Page();
	}
	
	public async Task<ActionResult> OnPost()
	{
		_logger.LogMethodEntered();

		try
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			
			switch (SelectedActionOrDecision)
			{
				case Options.Decision:
					var userName = GetUserName();
					var caseUrn = await _createCaseService.CreateNonConcernsCase(userName);
				
					return Redirect($"/case/{caseUrn}/management/action/decision/addOrUpdate");
				
				case Options.Action:
					return Redirect("/case/create/nonconcerns/action");

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			TempData["Error.Message"] = ErrorOnPostPage;
			
			return Page();
		}
	}

	public enum Options
	{
		Action,
		Decision
	}
	
	private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
}