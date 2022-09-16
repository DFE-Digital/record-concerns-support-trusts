﻿using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConcernsCasework.Service.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;

namespace ConcernsCaseWork.Pages.Case.Management.Action
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ISRMAService _srmaService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<IndexPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public List<CaseActionModel> CaseActions { get; private set; }

		public IndexPageModel(ICaseModelService caseModelService,
			ISRMAService srmaService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			ILogger<IndexPageModel> logger)
		{
			_caseModelService = caseModelService;
			_srmaService = srmaService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::Action::IndexPageModel::OnGetAsync");

				// Fetch case urn
				var caseUrn = GetRouteData();

				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::Action::IndexPageModel::OnPostAsync");

				// Fetch case urn
				var caseUrn = GetRouteData();

				// Form
				var action = Request.Form["action"].ToString();

				if (string.IsNullOrEmpty(action))
					throw new Exception("Missing form values");

				if (!Enum.TryParse(action.ToString(), out CaseActionEnum caseAction))
				{
					throw new Exception($"{action} - is not a recognized case action");
				}

				CaseActions = CaseActions ?? new List<CaseActionModel>();

				var actionStartHelpers = GetStartHelpers();

				var caseActionStarter = actionStartHelpers.SingleOrDefault(s => s.CanHandle(caseAction)) 
					?? throw new NotImplementedException($"{caseAction} - has not been implemented");

				if (await caseActionStarter.NewCaseActionAllowed(caseUrn, User.Identity.Name))
				{
					return RedirectToPage($"{action.ToLower()}/add", new { urn = caseUrn });
				}
				else
				{
					throw new InvalidOperationException($"Cannot create action of type {caseAction} for case {caseUrn}");
				}
			}
			catch (InvalidOperationException ex)
			{
				TempData["CaseAction.Error"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::IndexPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private List<CaseActionCreateHelper> GetStartHelpers()
		{
			return new List<CaseActionCreateHelper>
			{
				new SrmaCreateHelper(_srmaService),
				new FinancialPlanCreateHelper(_financialPlanModelService),
				new NtiCreateHelper(_ntiUnderConsiderationModelService, _ntiWarningLetterModelService, _ntiModelService)
			};
		}

		private long GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("Case::Action::IndexPageModel::OnGetAsync::CaseUrn is null or invalid to parse");
			}

			return caseUrn;
		}
	}
}