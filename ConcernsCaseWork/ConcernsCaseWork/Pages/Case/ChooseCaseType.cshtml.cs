using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using Service.Redis.Users;
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
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		
		[BindProperty(SupportsGet = true)]
		public string TrustName { get; set; }
		
		[BindProperty]
		[Required(ErrorMessage = "Case Type is required")]
		public string CaseType { get; set; }

		public ChooseCaseTypePageModel(ILogger<ChooseCaseTypePageModel> logger, IUserStateCachedService userStateCachedService, IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_logger = Guard.Against.Null(logger);
			_userStateCache = Guard.Against.Null(userStateCachedService);
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
				
				if (CaseType == "2")
				{
					throw new NotImplementedException();
				}
				
				var userName = GetUserName();
				var userState = await _userStateCache.GetData(userName) ?? new UserState(userName);

				userState.CreateCaseModel = new CreateCaseModel();

				await _userStateCache.StoreData(GetUserName(), userState);
				
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
			var userState = await _userStateCache.GetData(userName);

			TrustName = userState?.TrustName 
			            ?? throw new NullReferenceException($"Could not retrieve trust name from cache for user '{userName}'");
		}

		private string GetUserName()
			=> _claimsPrincipalHelper.GetPrincipalName(User) ?? throw new Exception("Could not retrieve current user");
	}
}