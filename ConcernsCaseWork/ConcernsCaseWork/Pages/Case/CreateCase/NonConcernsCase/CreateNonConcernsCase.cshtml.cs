using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Users;
using System;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CreateNonConcernsCasePageModel : AbstractPageModel
{
	private readonly IUserStateCachedService _cachedService;
	private readonly ILogger<CreateNonConcernsCasePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

	[BindProperty]
	public Options SelectedActionOrDecision { get; set; }

	public Actions SelectedAction { get; set; }

	public CreateNonConcernsCasePageModel(
		IUserStateCachedService cachedService,
		ILogger<CreateNonConcernsCasePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper)
	{
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}
	
	public ActionResult OnPost()
	{
		_logger.LogMethodEntered();
			
		try
		{
			if (SelectedActionOrDecision == Options.Decision)
			{
				return Redirect("/Decision");
			}

			switch (SelectedAction)
			{
				case Actions.TFF:
					return Redirect("/Decision");
				
				case Actions.SRMA:
					return Redirect("/Decision");
				
				case Actions.None:
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}

			return Page();
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
		None,
		Action,
		Decision
	}
	
	public enum Actions
	{
		None,
		SRMA,
		TFF
	}
}