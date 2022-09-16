using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Services.NtiUnderConsideration;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly INtiUnderConsiderationReasonsCachedService _ntiReasonsCachedService;
		private readonly ILogger<AddPageModel> _logger;
		

		public const int NotesMaxLength = 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsider;

		public long CaseUrn { get; private set; }

		public AddPageModel(
			INtiUnderConsiderationModelService ntiModelService,
			INtiUnderConsiderationReasonsCachedService ntiReasonsCachedService,
			ILogger<AddPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiReasonsCachedService = ntiReasonsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::AddPageModel::OnGetAsync");

			try
			{
				NTIReasonsToConsider = await GetReasons();
				ExtractCaseUrnFromRoute();

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}

		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				ExtractCaseUrnFromRoute();

				var newNti = PopulateNtiFromRequest();

				await _ntiModelService.CreateNti(newNti);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch(InvalidUserInputException ex)
			{
				TempData["NTI-UC.Message"] = ex.Message;
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private void ExtractCaseUrnFromRoute()
		{
			if (TryGetRouteValueInt64("urn", out var caseUrn))
			{
				CaseUrn = caseUrn;
			}
			else
			{
				throw new Exception("CaseUrn not found in the route");
			}
		}

		private async Task<IEnumerable<RadioItem>> GetReasons()
		{
			var reasons = await _ntiReasonsCachedService.GetAllReasons();
			return reasons.Select(r => new RadioItem
						   {
							   Id = Convert.ToString(r.Id),
							   Text = r.Name
						   });
		}

		private NtiUnderConsiderationModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			
			var nti = new NtiUnderConsiderationModel() { CaseUrn = CaseUrn };
			nti.NtiReasonsForConsidering = reasons.Select(r => new NtiReasonForConsideringModel { Id = int.Parse(r) }).ToArray();

			var notes = Convert.ToString(Request.Form["nti-notes"]);

			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new InvalidUserInputException($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
				else
				{
					nti.Notes = notes;
				}
			}

			return nti;
		}
	}
}