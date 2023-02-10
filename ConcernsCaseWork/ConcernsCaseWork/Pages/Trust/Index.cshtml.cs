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
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Base;
using System.Linq;

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
		public FindTrustModel FindTrustModel { get; set; }

		public IndexPageModel(ITrustModelService trustModelService, IUserStateCachedService userStateCache, ILogger<IndexPageModel> logger, IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			FindTrustModel = new();
		}

		/// <summary>
		/// This method takes a search query and returns the trusts that are (partial) matches, based on group name, ukPrn and companies house number
		/// </summary>
		/// <param name="searchQuery"></param>
		/// <param name="nonce"></param>
		/// <returns></returns>

		public async Task<ActionResult> OnGetTrustsSearchResult(string searchQuery, string nonce)
		{
			try
			{
				_logger.LogMethodEntered();


				// Double check search query.
				if (string.IsNullOrEmpty(searchQuery) || searchQuery.Length < _searchQueryMinLength)
				{
					_logger.LogInformationMsg($"Search rejected, searchQuery too short");
					return new JsonResult(new TrustSearchResponse() { Nonce = nonce });
				}

				_logger.LogInformationMsg($"Entered trust search: searchQuery -'{searchQuery}', nonce-{nonce}");
				var trustSearch = new TrustSearch(searchQuery, searchQuery, searchQuery);
				var trustSearchResponse = await _trustModelService.GetTrustsBySearchCriteria(trustSearch);

				return new JsonResult(new TrustSearchResponse() { Nonce = nonce, Data = trustSearchResponse.Data, TotalMatchesFromApi = trustSearchResponse.PageData.TotalMatchesFromApi, IsMoreDataOnServer = trustSearchResponse.PageData.IsMoreDataOnServer });
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnGetTrustsSearchResult::Exception - {Message}", ex.Message);

				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}


		public async Task<ActionResult> OnPost()
		{
			_logger.LogMethodEntered();

			if (!ModelState.IsValid)
			{
				TempData["Error.Message"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
				return Page();
			}

			try
			{
				_logger.LogInformation("Trust::IndexPageModel::OnPost");

				if (string.IsNullOrEmpty(FindTrustModel?.SelectedTrustUkprn) ||
					FindTrustModel.SelectedTrustUkprn.Contains("-") ||
					FindTrustModel.SelectedTrustUkprn.Length < _searchQueryMinLength)
				{
					throw new Exception($"Selected trust is incorrect - {FindTrustModel?.SelectedTrustUkprn}");
				}

				// Store CaseState into cache.
				var userState = await _userStateCache.GetData(GetUserName()) ?? new UserState(GetUserName());
				userState.TrustUkPrn = FindTrustModel.SelectedTrustUkprn;
				userState.CreateCaseModel = new CreateCaseModel();
				await _userStateCache.StoreData(GetUserName(), userState);

				return Redirect(Url.Page("Overview", new { id = FindTrustModel.SelectedTrustUkprn }));
			}
			catch (Exception ex)
			{
				_logger.LogError("Trust::IndexPageModel::OnPost::Exception - {Message}", ex.Message);

				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}