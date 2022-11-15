using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Cases.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddSrmaPageModel : AbstractPageModel
	{
		private readonly ILogger<AddSrmaPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ICreateCaseService _createCaseService;
		private const int _notesMaxLength = 500;
		
		public int NotesMaxLength => _notesMaxLength;
		public IEnumerable<RadioItem> SRMAStatuses => GetStatuses();
		
		[BindProperty]
		[Required (ErrorMessage = "Select status")]
		public SRMAStatus Status { get; set; }
		
		[BindProperty]
		[Required (ErrorMessage = "Enter a valid date")]
		public ConcernsDateValidatable OfferedDate { get; set; }

		[MaxLength(_notesMaxLength)]
		public string Notes { get; set; }
		
		public AddSrmaPageModel(
			IClaimsPrincipalHelper claimsPrincipalHelper, 
			ICreateCaseService createCaseService, 
			ILogger<AddSrmaPageModel> logger)
		{
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_createCaseService = Guard.Against.Null(createCaseService);
			_logger = Guard.Against.Null(logger);
		}

		public IActionResult OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					return Page();
				}
				
				var userName = GetUserName();
				var srma = CreateSrma();
				
				var caseUrn = await _createCaseService.CreateNonConcernsCase(userName, srma);
				
				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private static IEnumerable<RadioItem> GetStatuses()
		{
			var statuses = (SRMAStatus[])Enum.GetValues(typeof(SRMAStatus));
			return statuses.Where(s => s != SRMAStatus.Unknown && s != SRMAStatus.Declined && s != SRMAStatus.Canceled && s != SRMAStatus.Complete)
						   .Select(s => new RadioItem
						   {
							   Id = s.ToString(),
							   Text = EnumHelper.GetEnumDescription(s)
						   });
		}

		private SRMAModel CreateSrma()
		{
			var srma = new SRMAModel(
				0,
				0,
				DateOfferedToDate(),
				null,
				null,
				null,
				null,
				Status,
				Notes,
				SRMAReasonOffered.Unknown,
				DateTime.Now
				);

			return srma;
		}
		
		private DateTime DateOfferedToDate() => DateTimeHelper.ParseExact(OfferedDate.ToString());
		
		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}