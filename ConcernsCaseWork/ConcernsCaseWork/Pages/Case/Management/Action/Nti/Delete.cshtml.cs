using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using ConcernsCaseWork.Logging;
using System.Threading.Tasks;
using ConcernsCaseWork.Services.Nti;
using System.Net.Http;
using Microsoft.FeatureManagement.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[FeatureGate(ConcernsCaseWork.API.Contracts.Configuration.FeatureFlags.IsNtiDeletePageEnabled)]
	public class DeletePageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;

		private readonly ILogger<DeletePageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(SupportsGet = true, Name = "NtiId")]
		public long NtiId { get; set; }


		public DeletePageModel(
			INtiModelService ntiModelService,
			ILogger<DeletePageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				var model = await _ntiModelService.GetNtiByIdAsync(NtiId);
				if (model == null)
				{
					return NotFound();
				}
				if (model != null && model.CaseUrn != this.CaseUrn)
				{
					return NotFound();
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogErrorMsg(ex);
				if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return NotFound();
				}
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
					return Page();
				}

				await _ntiModelService.DeleteAsync(NtiId);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
	}
}
