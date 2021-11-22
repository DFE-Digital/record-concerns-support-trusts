using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ViewClosedPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<ViewClosedPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		
		// TODO Re-think about Dictionary on this page, maybe we just need a list of typeModel
		public IDictionary<long, TypeModel> TypeModelMap { get; private set; }
		
		public ViewClosedPageModel(ICaseModelService caseModelService, 
			ITrustModelService trustModelService, 
			ITypeModelService typeModelService,
			ILogger<ViewClosedPageModel> logger)
		{
			_caseModelService = caseModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ViewClosedPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("ViewClosedPageModel::CaseUrn is null or invalid to parse");
				}

				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
				
				// TODO Integrate records model service and build a IDic<long, TypeModel>
				var typeModel = await _typeModelService.GetTypeModelByUrn(CaseModel.TypeUrn);
				TypeModelMap = new Dictionary<long, TypeModel>{ { CaseModel.RecordUrn, typeModel } };
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ViewClosedPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}