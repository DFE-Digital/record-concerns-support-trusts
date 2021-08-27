using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trust;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class CaseModel : PageModel
	{
		private readonly ILogger<CaseModel> _logger;
		private readonly ITrustModelService _trustModelService;
		
		public IList<TrustModel> TrustSearch { get; private set; }
		
		public CaseModel(ITrustModelService trustModelService, ILogger<CaseModel> logger)
		{
			_trustModelService = trustModelService;
			_logger = logger;
		}
		
		public void OnGet(string searchQuery)
		{
			if (!string.IsNullOrEmpty(searchQuery)) {
				TrustSearch = new List<TrustModel>
				{
					new TrustModel { Id = 1, Name = "Test1"},
					new TrustModel { Id = 2, Name = "Test2"},
					new TrustModel { Id = 3, Name = "Test3"},
					new TrustModel { Id = 4, Name = "Test4"}
				};
			}
		}

		public async Task OnPostAsync(string searchQuery)
		{
			TrustSearch = new List<TrustModel>
			{
				new TrustModel { Id = 1, Name = "Test1"},
				new TrustModel { Id = 2, Name = "Test2"},
				new TrustModel { Id = 3, Name = "Test3"},
				new TrustModel { Id = 4, Name = "Test4"}
			};
		}
	}
}