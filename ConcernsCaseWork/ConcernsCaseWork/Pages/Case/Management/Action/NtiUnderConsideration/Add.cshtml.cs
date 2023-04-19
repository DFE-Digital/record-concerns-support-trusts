﻿using ConcernsCaseWork.API.Contracts.Constants;
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
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Helpers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ILogger<AddPageModel> _logger;


		public const int NotesMaxLength = 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsider;

		public long CaseUrn { get; private set; }
		private int _max;
		
		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }


		public AddPageModel(
			INtiUnderConsiderationModelService ntiModelService,
			ILogger<AddPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
			_max = NtiConstants.MaxNotesLength;
		}

		public IActionResult OnGet()
		{
			_logger.LogInformation("Case::Action::NTI-UC::AddPageModel::OnGetAsync");

			try
			{
				LoadPageComponents();
				NTIReasonsToConsider = GetReasons();
				ExtractCaseUrnFromRoute();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::AddPageModel::OnGet::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					var data = PopulateNtiFromRequest();
					NTIReasonsToConsider = GetReasons();
					
					ExtractCaseUrnFromRoute();
					return Page();
				}
				
				ExtractCaseUrnFromRoute();
				var newNti = PopulateNtiFromRequest();
				await _ntiModelService.CreateNti(newNti);
				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (InvalidUserInputException ex)
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

		private IEnumerable<RadioItem> GetReasons()
		{
			var reasonValues = Enum.GetValues<NtiUnderConsiderationReason>().ToList();

			return reasonValues.Select(r => new RadioItem
			{
				Id = Convert.ToString((int)r),
				Text = EnumHelper.GetEnumDescription(r)
			});
		}

		private NtiUnderConsiderationModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			var nti = new NtiUnderConsiderationModel() { CaseUrn = CaseUrn };
			nti.NtiReasonsForConsidering = reasons.Select(r => new NtiReasonForConsideringModel { Id = int.Parse(r) }).ToArray();
			nti.Notes = Notes.Text.StringContents;
			return nti;
		}
		
		private void LoadPageComponents()
		{
			Notes = BuildNotesComponent();
		}
		
		private TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("nti-notes", nameof(Notes), "Notes (optional)")
			{
				HintText = "Case owners can record any information they want that feels relevant to the action",
				Text = new ValidateableString()
				{
					MaxLength = _max,
					StringContents = contents,
					DisplayName = "Notes"
				}
			};
		
		private void ResetOnValidationError()
		{
			
			Notes = BuildNotesComponent(Notes.Text.StringContents);
		}

		
	}
}