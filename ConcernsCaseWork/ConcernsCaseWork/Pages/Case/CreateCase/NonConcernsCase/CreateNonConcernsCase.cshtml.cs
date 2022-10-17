using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Users;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CreateNonConcernsCasePageModel : AbstractPageModel
{
	private readonly IUserStateCachedService _cachedService;
	private readonly ILogger<CreateNonConcernsCasePageModel> _logger;
	private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
	private readonly ICreateCaseService _createCaseService;

	[BindProperty]
	public Options SelectedActionOrDecision { get; set; }

	[BindProperty]
	public Actions SelectedAction { get; set; }

	public CreateNonConcernsCasePageModel(
		IUserStateCachedService cachedService,
		ILogger<CreateNonConcernsCasePageModel> logger,
		IClaimsPrincipalHelper claimsPrincipalHelper,
		ICreateCaseService createCaseService)
	{
		_createCaseService = createCaseService;
		_cachedService = Guard.Against.Null(cachedService);
		_logger = Guard.Against.Null(logger);
		_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
	}
	
	public async Task<ActionResult> OnPost()
	{
		_logger.LogMethodEntered();

		long caseUrn = 0;
		try
		{
			if (SelectedActionOrDecision == Options.Decision)
			{
				var userName = GetUserName();
				caseUrn = await _createCaseService.CreateNonConcernsCase(userName);
				
				return Redirect($"/case/{caseUrn}/management/action/decision/add");
			}

			switch (SelectedAction)
			{
				case Actions.TFF:
					throw new NotImplementedException();
				
				case Actions.SRMA:
					var userName = GetUserName();
					caseUrn = await _createCaseService.CreateNonConcernsCase(userName);
					
					return Redirect($"/case/{caseUrn}/management/action/srma/add");
				
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
	
	private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
}