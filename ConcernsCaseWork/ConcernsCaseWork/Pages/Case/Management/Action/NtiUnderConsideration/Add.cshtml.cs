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

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
		private readonly ILogger<AddPageModel> _logger;
		

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsider => GetReasons();

		public long CaseUrn { get; private set; }

		public AddPageModel(
			INtiModelService ntiModelService,
			ISRMAService srmaModelService, ILogger<AddPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::AddPageModel::OnGetAsync");

			try
			{
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
			catch (InvalidOperationException ex)
			{
				TempData["SRMA.Message"] = ex.Message;
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
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private IEnumerable<RadioItem> GetReasons()
		{
			var statuses = (NtiReasonForConsidering[])Enum.GetValues(typeof(NtiReasonForConsidering));
			return statuses.Where(r => r != NtiReasonForConsidering.None)
						   .Select(s => new RadioItem
						   {
							   Id = s.ToString(),
							   Text = EnumHelper.GetEnumDescription(s)
						   });
		}

		private NtiModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			var reasonsStr = string.Join("," ,reasons);

			var nti = new NtiModel() { CaseUrn = CaseUrn };
			if (Enum.TryParse<NtiReasonForConsidering>(reasonsStr, out var reasonsEnum))
			{
				nti.NtiReasonForConsidering = reasonsEnum;
			}
			// else: no validation necessary - reason is not a mandatory field atm

			var notes = Convert.ToString(Request.Form["nti-notes"]);

			if (!string.IsNullOrEmpty(notes))
			{
				if (notes.Length > NotesMaxLength)
				{
					throw new Exception($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
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