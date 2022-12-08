using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.Trusts;
using System;
using System.Net;
using System.Threading.Tasks;
using ConcernsCaseWork.Pages.Base;

namespace ConcernsCaseWork.Pages.Trust
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		private const int _searchQueryMinLength = 3;

		[BindProperty]
		[HiddenInput]
		public string SelectedTrustUkprn { get; set; }

		[BindProperty]
		public FindTrustModel FindTrustModel { get; set; }

		public IndexPageModel(ITrustModelService trustModelService, IUserStateCachedService userStateCache, ILogger<IndexPageModel> logger, IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			FindTrustModel = new();
		}

		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery)
		{
			try
			{
				_logger.LogInformation("Trust::IndexPageModel::OnGetTrustsPartial");

				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < _searchQueryMinLength)
				{
					return new JsonResult(Array.Empty<TrustSearchModel>());
				}

				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);

				return new JsonResult(trustSearchResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnGetTrustsSearchResult::Exception - {Message}", ex.Message);

				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}


		public async Task<ActionResult> OnPost(string selectedTrustUkprn)
		{
			_logger.LogMethodEntered();

			try
			{
				string trustUkPrn = SelectedTrustUkprn;
				_logger.LogInformation("Trust::IndexPageModel::OnPost");

				// Double check selected trust.;
				if (string.IsNullOrEmpty(trustUkPrn) || trustUkPrn.Contains("-"))
				{
					throw new Exception($"Trust::IndexModel::OnPost::Selected trust is incorrect - {trustUkPrn}");
				}

				// Store CaseState into cache.
				var userState = await _userStateCache.GetData(GetUserName()) ?? new UserState(GetUserName());
				userState.TrustUkPrn = trustUkPrn;
				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(GetUserName(), userState);

				return Redirect(Url.Page("Overview", new { id = trustUkPrn }));

				//return new JsonResult(new { redirectUrl = Url.Page("Overview", new { id = trustUkPrn }) });
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnPost::Exception - {Message}", ex.Message);

				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<ActionResult> OnGetCancel()
		{
			try
			{
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::AddPageModel::OnGetCancel::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}