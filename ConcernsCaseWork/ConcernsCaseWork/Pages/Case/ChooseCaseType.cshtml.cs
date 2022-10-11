using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ChooseCaseTypePageModel : AbstractPageModel
	{
		private readonly ILogger<ChooseCaseTypePageModel> _logger;
		private readonly ICreateCaseService _createCaseService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		
		[BindProperty(SupportsGet = true)]
		public TrustAddressModel TrustAddress { get; set; }
		
		[BindProperty]
		[Required(ErrorMessage = "Case Type is required")]
		public int CaseType { get; set; }

		public ChooseCaseTypePageModel(ILogger<ChooseCaseTypePageModel> logger, ICreateCaseService createCaseService, IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_logger = Guard.Against.Null(logger);
			_createCaseService = Guard.Against.Null(createCaseService);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
		}

		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();
			
			try
			{
				await InitPage();
			}
			catch (Exception ex)
            {
                _logger.LogErrorMsg(ex);
  
                TempData["Error.Message"] = ErrorOnGetPage;
            }
			
            return Page();
		}

		public async Task<ActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();
			
			try
			{
				if (!ModelState.IsValid)
				{
					await InitPage();
					return Page();
				}
				
				var userName = GetUserName();
				
				await _createCaseService.SetCaseTypeInNewCaseWizard(userName, CaseType);
				
				return RedirectToPage("Concern/Index");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				TempData["Error.Message"] = ErrorOnPostPage;
				return RedirectToPage("choosecasetype");
			}
		}

		private async Task InitPage()
		{
			var userName = GetUserName();
			var trustAddress = await _createCaseService.GetSelectedTrustAddress(userName);

			TrustAddress = trustAddress 
			               ?? throw new NullReferenceException($"Could not retrieve trust from cache for user '{userName}'");
		}

		private string GetUserName()
			=> _claimsPrincipalHelper.GetPrincipalName(User) ?? throw new Exception("Could not retrieve current user");
	}
}