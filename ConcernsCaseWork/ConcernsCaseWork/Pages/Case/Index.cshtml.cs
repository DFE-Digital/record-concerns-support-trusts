using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Trusts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : PageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ICreateCaseService _createCaseService;

		private const int SearchQueryMinLength = 3;
		
		public IndexPageModel(ITrustModelService trustModelService, 
			ILogger<IndexPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			ICreateCaseService createCaseService)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_createCaseService = Guard.Against.Null(createCaseService);
		}
		
		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogMethodEntered();
				
				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < SearchQueryMinLength)
					return new JsonResult(Array.Empty<TrustSearchModel>());

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);

				return new JsonResult(trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		
		public async Task<ActionResult> OnGetSelectedTrust(string trustUkPrn, string trustName)
		{
			try
			{
				_logger.LogMethodEntered();
				
				// Double check selected trust.
				if (string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains("-") || trustUkPrn.Length < SearchQueryMinLength)
					throw new Exception($"Selected trust is incorrect - {trustUkPrn}");
				
				// Store CaseState into cache.
				var userName = GetUserName();

				await _createCaseService.StartCreateNewCaseWizard(userName);
				await _createCaseService.SetTrustInCreateCaseWizard(userName, trustUkPrn);
		
				return new JsonResult(new { redirectUrl = "/case/choosecasetype" });
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
					
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}