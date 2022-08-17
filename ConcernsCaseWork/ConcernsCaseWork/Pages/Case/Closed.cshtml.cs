using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConcernsCasework.Service.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosedPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<ClosedPageModel> _logger;
		
		public IList<HomeModel> CasesClosed { get; private set; }
		
		public ClosedPageModel(ICaseModelService caseModelService, ILogger<ClosedPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("ClosedPageModel::OnGetAsync executed");
		        
				CasesClosed = await _caseModelService.GetCasesByCaseworkerAndStatus(User.Identity.Name, StatusEnum.Close);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosedPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				CasesClosed = Array.Empty<HomeModel>();
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
	}
}