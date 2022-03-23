using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseActions = ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ISRMAService _srmaService;
		private readonly ILogger<IndexPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public List<CaseAction> CaseActions { get; private set; }

		public IndexPageModel(ICaseModelService caseModelService,
			ISRMAService srmaService,
			ILogger<IndexPageModel> logger)
		{
			_caseModelService = caseModelService;
			_srmaService = srmaService;
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

				CaseActions = CaseActions ?? new List<CaseAction>();

				switch (caseAction)
				{
					case CaseActionEnum.Srma:
					
						CaseActions.AddRange(await _srmaService.GetSRMAsForCase(caseUrn));

						//Check if case action is SRMA and status is not deployed
						var openSrma = CaseActions.Where(ca => ca is CaseActions.SRMA && !(((CaseActions.SRMA)ca).Status.CompareTo(SRMAStatus.Deployed) == 0)).FirstOrDefault();

						if (openSrma != null)
							throw new ApplicationException("There is already an open SRMA action linked to this case. Please resolve that before opening another one.");
						break;
					default:
						throw new NotImplementedException($"{caseAction} - has not been implemented");
						break;
				}

				return RedirectToPage($"{action.ToLower()}/add", new { urn = caseUrn });
			}
			catch (ApplicationException ex)
			{
				_logger.LogError("Case::Action::IndexPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["CaseAction.Error"] = ex.Message;
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::IndexPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
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