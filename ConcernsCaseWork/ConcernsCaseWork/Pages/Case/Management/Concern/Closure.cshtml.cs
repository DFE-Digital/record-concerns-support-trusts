using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService; 
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ILogger<ClosurePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public TypeModel TypeModel { get; private set; }

		public ClosurePageModel(ICaseModelService caseModelService, 
			IRatingModelService ratingModelService, 
			IRecordModelService recordModelService,
			ITrustModelService trustModelService, 
			ITypeModelService typeModelService,
			IStatusCachedService statusCachedService,
			ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_statusCachedService = statusCachedService;
			_logger = logger;
		}
		
		public async Task OnGet()
		{
			long caseUrn = 0;
			long recordUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::Concern::ClosurePageModel::OnGet");
				(caseUrn, recordUrn) = GetRouteData();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::ClosurePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordUrn);
		}

		public async Task<ActionResult> OnGetCloseConcern()
		{
			long caseUrn = 0;
			long recordUrn = 0;

			try
			{
				_logger.LogInformation("Case::Concern::ClosurePageModel::CloseConcern");

				(caseUrn, recordUrn) = GetRouteData();
				var closedStatus = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());
				
				var currentDate = DateTimeOffset.Now;
				var patchRecordModel = new PatchRecordModel()
				{
					Id = recordUrn,
					CaseUrn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = currentDate,
					ClosedAt = currentDate,
					StatusId = closedStatus.Id
				};

				await _recordModelService.PatchRecordStatus(patchRecordModel);

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::ClosurePageModel::CloseConcern::OnGetCloseConcern::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordUrn);
		}

		public ActionResult OnGetCancel()
		{
			try
			{
				long caseUrn;
				(caseUrn, _) = GetRouteData();

				var url = $"/case/{caseUrn}/management";
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::ClosurePageModel::OnGetCancel::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long recordUrn)
		{
			try
			{
				if (caseUrn == 0 || recordUrn == 0)
					throw new Exception("Case urn or record urn cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				var recordModel = await _recordModelService.GetRecordModelById(User.Identity.Name, caseUrn, recordUrn);
				RatingsModel = await _ratingModelService.GetSelectedRatingsModelById(recordModel.RatingId);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				TypeModel = await _typeModelService.GetSelectedTypeModelById(recordModel.TypeId);
				CaseModel.PreviousUrl = url;

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Concern::ClosurePageModel::LoadPage::Exception - {Message}", ex.Message);
					
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		} 

		private (long caseUrn, long recordUrn) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var recordUrnValue = RouteData.Values["recordUrn"];
			if (recordUrnValue == null || !long.TryParse(recordUrnValue.ToString(), out long recordUrn) || recordUrn == 0)
				throw new Exception("RecordUrn is null or invalid to parse");

			return (caseUrn, recordUrn);
		}
	}
}