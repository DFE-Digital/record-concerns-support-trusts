using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		[BindProperty]
		public RadioButtonsUiComponent SRMAStatus { get; set; }

		[BindProperty]
		public OptionalDateTimeUiComponent DateOffered { get; set; }

		[BindProperty]
		public TextAreaUiComponent Notes { get; set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		public AddPageModel(
			ISRMAService srmaModelService, ILogger<AddPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public IActionResult OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				LoadPageComponents();
				return Page();
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
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					ResetOnValidationError();
					return Page();
				}

				var srma = CreateSRMA(CaseUrn);
				await _srmaModelService.SaveSRMA(srma);

				return Redirect($"/case/{srma.CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents()
		{
			SRMAStatus = BuildStatusComponent();
			DateOffered = BuildDateOfferedComponent(new OptionalDateModel());
			Notes = BuildNotesComponent();
		}

		private void ResetOnValidationError()
		{
			SRMAStatus = BuildStatusComponent(SRMAStatus.SelectedId);
			DateOffered = BuildDateOfferedComponent(DateOffered.Date);
			Notes = BuildNotesComponent(Notes.Text.StringContents);
		}

		private static RadioButtonsUiComponent BuildStatusComponent(int? selectedId = null)
		{
			var enumValues = new List<SRMAStatus>()
			{
				API.Contracts.Srma.SRMAStatus.TrustConsidering,
				API.Contracts.Srma.SRMAStatus.PreparingForDeployment,
				API.Contracts.Srma.SRMAStatus.Deployed
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "srma-status", Name: nameof(SRMAStatus), "What is the status of the SRMA?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "SRMA status",
				Required = true
			};
		}

		private static OptionalDateTimeUiComponent BuildDateOfferedComponent(OptionalDateModel date)
		{
			return new OptionalDateTimeUiComponent("date-offered", nameof(DateOffered), "When was the trust contacted?")
			{
				Date = date,
				Required = true,
				DisplayName = "Date trust was contacted"
			};
		}

		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("srma-notes", nameof(Notes), "Notes (optional)")
			{
				HintText = "Case owners can record any information they want that feels relevant to the action.",
				Text = new ValidateableString()
				{
					MaxLength = SrmaConstants.NotesLength,
					StringContents = contents,
					DisplayName = "Notes"
				}
			};

		private SRMAModel CreateSRMA(long caseUrn)
		{
			var now = DateTime.Now;
			var createdBy = User.Identity.Name;

			var srma = new SRMAModel(
				0,
				caseUrn,
				DateOffered.Date.ToDateTime().Value,
				null,
				null,
				null,
				null,
				(SRMAStatus)SRMAStatus.SelectedId,
				Notes.Text.StringContents,
				SRMAReasonOffered.Unknown,
				now,
				now,
				createdBy
				);

			return srma;
		}
	}
}