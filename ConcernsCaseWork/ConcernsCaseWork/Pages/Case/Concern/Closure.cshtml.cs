﻿using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly IStatusService _statusService;
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
			IStatusService statusService,
			ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_statusService = statusService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
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
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordUrn);
		}

		public async Task<ActionResult> OnGetCloseConcern()
		{
			long caseUrn = 0;
			long recordUrn = 0;

			try
			{
				_logger.LogInformation("Case::Concern::ClosurePageModel::CloseConcern");

				(caseUrn, recordUrn) = GetRouteData();
				var statuses = await _statusService.GetStatuses();
				var closedStatus = statuses.FirstOrDefault(s => s.Name.CompareTo(StatusEnum.Close.ToString()) == 0);

				var currentDate = DateTimeOffset.Now;
				var patchRecordModel = new PatchRecordModel()
				{
					Urn = recordUrn,
					CaseUrn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = currentDate,
					ClosedAt = currentDate,
					StatusUrn = closedStatus.Urn
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

			return await LoadPage("", caseUrn, recordUrn);
		}

		public ActionResult OnGetCancel()
		{
			long caseUrn = 0;
			long recordUrn = 0;

			try
			{
				(caseUrn, recordUrn) = GetRouteData();

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
			if (caseUrn == 0 || recordUrn == 0) return Page();
			
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			var recordModel = await _recordModelService.GetRecordModelByUrn(User.Identity.Name, caseUrn, recordUrn);
			RatingsModel = await _ratingModelService.GetSelectedRatingsModelByUrn(recordModel.RatingUrn);
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			TypeModel = await _typeModelService.GetSelectedTypeModelByUrn(recordModel.TypeUrn);
			CaseModel.PreviousUrl = url;

			return Page();
		} 

		private (long caseUrn, long recordUrn) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("Case::Concern::ClosurePageModel::CaseUrn is null or invalid to parse");
			}

			var recordUrnValue = RouteData.Values["recordUrn"];
			if (recordUrnValue == null || !long.TryParse(recordUrnValue.ToString(), out long recordUrn) || recordUrn == 0)
			{
				throw new Exception("Case::Concern::ClosurePageModel::RecordUrn is null or invalid to parse");
			}

			return (caseUrn, recordUrn);
		}
	}
}