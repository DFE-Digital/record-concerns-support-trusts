using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly ICachedService _cachedService;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		
		public AddPageModel(ITrustModelService trustModelService, 
			ICachedService cachedService,
			ILogger<AddPageModel> logger)
		{
			_trustModelService = trustModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::Concern::AddPageModel::OnGetAsync");
				
				// Fetch UI data
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::AddPageModel::Concern::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public ActionResult OnGetConcern()
		{
			return RedirectToPage("index");
		}
		
		public async Task<ActionResult> OnGetCancelCase()
		{
			var userState = await GetUserState();
			userState.CreateCaseModel = new CreateCaseModel();
			await _cachedService.StoreData(User.Identity.Name, userState);
			
			return Redirect("/");
		}

		private async Task<ActionResult> LoadPage()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;
			
			if (string.IsNullOrEmpty(trustUkPrn)) return Page();
			
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			
			return Page();
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData<UserState>(User.Identity.Name);
			if (userState is null)
				throw new Exception("Case::Concern::AddPageModel::Cache CaseStateData is null");
			
			return userState;
		}
	}
}