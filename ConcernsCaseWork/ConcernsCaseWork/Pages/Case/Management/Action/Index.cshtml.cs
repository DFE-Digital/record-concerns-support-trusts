using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly ISRMAService _srmaService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiModelService _ntiModelService;
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<IndexPageModel> _logger;


		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent CaseActionEnum { get; set; }

		public IndexPageModel(ICaseModelService caseModelService,
			IRecordModelService recordModelService,
			ISRMAService srmaService,
			IFinancialPlanModelService financialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiModelService ntiModelService,
			ITrustFinancialForecastService trustFinancialForecastService,
			ILogger<IndexPageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_srmaService = srmaService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiModelService = ntiModelService;
			_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();

					return Page();
				}

				var caseAction = (CaseActionEnum)CaseActionEnum.SelectedId;

				var actionStartHelpers = GetStartHelpers();

				var caseActionStarter = actionStartHelpers.SingleOrDefault(s => s.CanHandle(caseAction))
					?? throw new NotImplementedException($"{caseAction} - has not been implemented");

				if (await caseActionStarter.NewCaseActionAllowed(CaseId))
				{
					return RedirectToPage($"{caseAction.ToString().ToLower()}/add", new { urn = CaseId });
				}
				else
				{
					throw new InvalidOperationException($"Cannot create action of type {caseAction} for case {CaseId}");
				}
			}
			catch (InvalidOperationException ex)
			{
				await LoadPage();
				ModelState.AddModelError($"{nameof(CaseActionEnum)}.{CaseActionEnum.DisplayName}", ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private async Task LoadPage()
		{
			var caseModel = await _caseModelService.GetCaseByUrn(CaseId);
			caseModel.RecordsModel = await _recordModelService.GetRecordsModelByCaseUrn(CaseId);

			var supportedActions = GetSupportedActions(caseModel);
			CaseActionEnum = BuildActionComponent(supportedActions);
		}

		private List<CaseActionEnum> GetSupportedActions(CaseModel caseModel)
		{
			if (caseModel.RecordsModel.IsNullOrEmpty())
			{
				return new List<CaseActionEnum>()
				{
					Service.Cases.CaseActionEnum.Decision,
					Service.Cases.CaseActionEnum.Srma,
					Service.Cases.CaseActionEnum.TrustFinancialForecast
				};
			}

			if (caseModel.Division == Division.RegionsGroup)
			{
				return new List<CaseActionEnum>()
				{
					Service.Cases.CaseActionEnum.Decision,
					Service.Cases.CaseActionEnum.NtiUnderConsideration,
					Service.Cases.CaseActionEnum.NtiWarningLetter,
					Service.Cases.CaseActionEnum.Nti,
					Service.Cases.CaseActionEnum.Srma,
				};
			}

			return new List<CaseActionEnum>()
			{
				Service.Cases.CaseActionEnum.Decision,
				Service.Cases.CaseActionEnum.FinancialPlan,
				Service.Cases.CaseActionEnum.NtiUnderConsideration,
				Service.Cases.CaseActionEnum.NtiWarningLetter,
				Service.Cases.CaseActionEnum.Nti,
				Service.Cases.CaseActionEnum.Srma,
				Service.Cases.CaseActionEnum.TrustFinancialForecast
			};
		}

		private List<CaseActionCreateHelper> GetStartHelpers()
		{
			return new List<CaseActionCreateHelper>
			{
				new SrmaCreateHelper(_srmaService),
				new FinancialPlanCreateHelper(_financialPlanModelService),
				new NtiCreateHelper(_ntiUnderConsiderationModelService, _ntiWarningLetterModelService, _ntiModelService),
				new CaseDecisionCreateHelper(),
				new TrustFinancialForecastCreateHelper(_trustFinancialForecastService)
			};
		}

		private static RadioButtonsUiComponent BuildActionComponent(List<CaseActionEnum> caseActions)
		{
			var radioItems = caseActions.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "case-action", Name: nameof(CaseActionEnum), "What are you recording?")
			{
				RadioItems = radioItems,
				DisplayName = "an action or decision",
				Required = true
			};
		}
	}
}