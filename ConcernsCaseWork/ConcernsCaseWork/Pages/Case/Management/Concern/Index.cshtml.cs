using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<IndexPageModel> _logger;
		
		public TypeModel TypeModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }
		public string PreviousUrl { get; private set; }
		
		public IndexPageModel(ICaseModelService caseModelService,
			IRecordModelService recordModelService,
			ITrustModelService trustModelService,
			ITypeModelService typeModelService,
			IRatingModelService ratingModelService,
			ILogger<IndexPageModel> logger)
		{
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::Management::Concern::IndexPageModel::OnGetAsync");
					
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");
			
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Management::Concern::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			// Fetch UI data
			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<IActionResult> OnPostAsync(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::Management::Concern::IndexPageModel::OnPostAsync");

				if (!ConcernTypeValidator.IsValid(Request.Form))
					throw new Exception("Missing form values");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");
				
				string typeUrn;
				
				// Form
				var type = Request.Form["type"].ToString();
				var subType = Request.Form["sub-type"].ToString();
				var ragRating = Request.Form["rating"].ToString();
				
				// Type
				(typeUrn, type, subType) = type.SplitType(subType);

				// Rating
				var splitRagRating = ragRating.Split(":");
				var ragRatingUrn = splitRagRating[0];

				var createRecordModel = new CreateRecordModel
				{
					CaseUrn = caseUrn,
					TypeUrn = long.Parse(typeUrn),
					Type = type,
					SubType = subType,
					RatingUrn = long.Parse(ragRatingUrn)
				};
				
				// Post record
				await _recordModelService.PostRecordByCaseUrn(createRecordModel, User.Identity.Name);
				
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Management::Concern::IndexPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return await LoadPage(url, caseUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			try
			{
				if (caseUrn == 0) 
					throw new Exception("Case urn cannot be 0");
				
				// Get Case
				var caseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				
				PreviousUrl = url;
				CreateRecordsModel = await _recordModelService.GetCreateRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseModel.TrustUkPrn);
				RatingsModel = await _ratingModelService.GetRatingsModel();
				TypeModel = await _typeModelService.GetTypeModel();
			
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Management::Concern::IndexPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
	}
}