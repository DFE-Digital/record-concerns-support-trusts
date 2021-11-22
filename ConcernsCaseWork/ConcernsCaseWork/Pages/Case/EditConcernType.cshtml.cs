using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditConcernTypePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<EditConcernTypePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TypeModel TypeModel { get; private set; }
		
		public EditConcernTypePageModel(ITypeModelService typeModelService, ICaseModelService caseModelService, 
			ILogger<EditConcernTypePageModel> logger)
		{
			_typeModelService = typeModelService;
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			long recordUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditConcernTypePageModel::OnGetAsync");

				(caseUrn, recordUrn) = GetRouteData();

				if (caseUrn == 0 || recordUrn == 0) throw new Exception("Case::EditConcernTypePageModel missing route data");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditConcernTypePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn, recordUrn);
		}
		
		public async Task<ActionResult> OnPostEditConcernType(string url)
		{
			long caseUrn = 0;
			long recordUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditConcernTypePageModel::OnPostEditConcernType");
				
				if (!ConcernTypeValidator.IsEditValid(Request.Form))
					throw new Exception("Case::EditConcernTypePageModel::Missing form values");
				
				(caseUrn, recordUrn) = GetRouteData();
				
				string typeUrn;
				
				// Form
				var type = Request.Form["type"].ToString();
				var subType = Request.Form["sub-type"].ToString();
				
				// Type
				(typeUrn, type, subType) = type.SplitType(subType);
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					Type = type,
					SubType = subType,
					TypeUrn = long.Parse(typeUrn)
				};
					
				await _caseModelService.PatchConcernType(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditConcernTypePageModel::OnPostEditConcernType::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn, recordUrn);
		}

		private async Task<ActionResult> LoadPage(string url, long caseUrn, long recordUrn)
		{
			TypeModel = await _typeModelService.GetTypeModel();
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			CaseModel.PreviousUrl = url;
			
			return Page();
		}
		
		private (long caseUrn, long recordUrn) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn))
			{
				throw new Exception("Case::EditConcernTypePageModel::CaseUrn is null or invalid to parse");
			}

			var recordUrnValue = RouteData.Values["recordUrn"];
			if (recordUrnValue == null || !long.TryParse(recordUrnValue.ToString(), out long recordUrn))
			{
				throw new Exception("Case::EditConcernTypePageModel::RecordUrn is null or invalid to parse");
			}

			return (caseUrn, recordUrn);
		}
	}
}