﻿using ConcernsCaseWork.Enums;
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
using Service.Redis.Nti;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
		private readonly INtiReasonsCachedService _ntiReasonsCachedService;
		private readonly ILogger<EditPageModel> _logger;
		

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> NTIReasonsToConsiderForUI;

		public long CaseUrn { get; private set; }
		public NtiModel NtiModel { get; set; }

		public EditPageModel(
			INtiModelService ntiModelService,
			INtiReasonsCachedService ntiReasonsCachedService,
			ILogger<EditPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiReasonsCachedService = ntiReasonsCachedService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::NTI-UC::EditPageModel::OnGetAsync");

			try
			{
				CaseUrn = ExtractCaseUrnFromRoute();
				
				var ntiUcId = ExtractNtiUcIdFromRoute();
				NtiModel = await _ntiModelService.GetNtiUnderConsideration(ntiUcId);

				NTIReasonsToConsiderForUI = await GetReasonsForUI(NtiModel);

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
				CaseUrn = ExtractCaseUrnFromRoute();
				var ntiWithUpdatedValues = PopulateNtiFromRequest();

				await _ntiModelService.PatchNtiUnderConsideration(ntiWithUpdatedValues);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::NTI-UC::EditPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private long ExtractCaseUrnFromRoute()
		{
			if (TryGetRouteValueInt64("urn", out var caseUrn))
			{
				return caseUrn;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private long ExtractNtiUcIdFromRoute()
		{
			if (TryGetRouteValueInt64("ntiUCId", out var ntiUcId))
			{
				return ntiUcId;
			}
			else
			{
				throw new InvalidOperationException("CaseUrn not found in the route");
			}
		}

		private async Task<IEnumerable<RadioItem>> GetReasonsForUI(NtiModel ntiModel)
		{
			var reasons = await _ntiReasonsCachedService.GetAllReasons();
			return reasons.Select(r => new RadioItem
						   {
							   Id = Convert.ToString(r.Id),
							   Text = r.Name,
							   IsChecked = ntiModel?.NtiReasonsForConsidering?.Any(ntiR => ntiR.Id == r.Id) == true,	
						   });
		}

		private NtiModel PopulateNtiFromRequest()
		{
			var reasons = Request.Form["reason"];
			
			var nti = new NtiModel() { 
				Id = ExtractNtiUcIdFromRoute(),
				CaseUrn = CaseUrn,
			};

			nti.NtiReasonsForConsidering = reasons.Select(r => new NtiReasonForConsideringModel { Id = int.Parse(r) }).ToArray();

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