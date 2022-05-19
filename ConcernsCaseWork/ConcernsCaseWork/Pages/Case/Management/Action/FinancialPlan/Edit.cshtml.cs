using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.Redis.Models;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : AbstractPageModel
	{
		private readonly ILogger<EditPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;

		public FinancialPlanModel FinancialPlanModel { get; set; }
		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> FinancialPlanStatuses => getStatuses();

		public EditPageModel(
			IFinancialPlanModelService financialPlanModelService, ILogger<EditPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::EditPageModel::OnGetAsync");

			try
			{
				LoadPageData();

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			long caseUrn = 0;
			long? financialPlanId = null;

			try
			{
				(caseUrn, financialPlanId) = GetRouteData();
				LoadPageData();

				ValidateFinancialPlan();

				var notes = Request.Form["financial-plan-notes"];
				var currentUser = User.Identity.Name;

				if (financialPlanId.HasValue)
				{
					var patchFinancialPlanModel = new PatchFinancialPlanModel()
					{
						Id = financialPlanId.Value,
						CaseUrn = caseUrn,
						Notes = notes
					};


					//_financialPlanModelService.PatchFinancialPlanStatus
				}
				else
				{
					var createFinancialPlanModel = new CreateFinancialPlanModel
					{
						CaseUrn = caseUrn,
						CreatedAt = DateTime.Now,
						CreatedBy = currentUser,
						Notes = notes
					};

					// Post Financial Plan 
					await _financialPlanModelService.PostFinancialPlanByCaseUrn(createFinancialPlanModel, currentUser);
				}

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private IEnumerable<RadioItem> getStatuses()
		{
			var statuses = (FinancialPlanStatus[])Enum.GetValues(typeof(FinancialPlanStatus));
			return statuses.Where(f => f != FinancialPlanStatus.Unknown)
						   .Select(f => new RadioItem
						   {
							   Id = f.ToString(),
							   Text = EnumHelper.GetEnumDescription(f)
						   });
		}

		private (long, long?) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			var financialPlanIdValue = RouteData.Values["finanicialplanid"];

			long? financialPlanId = null;

			if (financialPlanIdValue != null)
			{
				if (!long.TryParse(financialPlanIdValue.ToString(), out long parsedValue) || parsedValue == 0)
				{
					throw new Exception("FinancialId is 0 or invalid to parse");
				}

				financialPlanId = parsedValue;
			}

			return (caseUrn, financialPlanId);
		}

		private void ValidateFinancialPlan()
		{
			var financial_plan_notes = Request.Form["financial-plan-notes"];

			if (!string.IsNullOrEmpty(financial_plan_notes))
			{
				var notes = financial_plan_notes.ToString();
				if (notes.Length > NotesMaxLength)
				{
					throw new InvalidOperationException($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}

		}

		private async void LoadPageData()
		{
			long caseUrn = 0;
			long? financialPlanId = null;

			try
			{
				(caseUrn, financialPlanId) = GetRouteData();

				if (financialPlanId.HasValue)
				{
					//var financialPlansModel = await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, User.Identity.Name);
					//FinancialPlanModel = financialPlansModel.FirstOrDefault(fp => fp.Id == financialPlanId.Value);
				}

			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}