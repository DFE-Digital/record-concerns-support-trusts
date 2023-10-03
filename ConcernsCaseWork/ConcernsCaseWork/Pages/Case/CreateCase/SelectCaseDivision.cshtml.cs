using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
	public class SelectCaseDivisionModel : CreateCaseBaseModel
    {
		private readonly ILogger<SelectCaseTypePageModel> _logger;

		[BindProperty]
		public RadioButtonsUiComponent CaseDivision { get; set; }

		public SelectCaseDivisionModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		IClaimsPrincipalHelper claimsPrincipalHelper,
		ILogger<SelectCaseTypePageModel> logger) : base(trustModelService, cachedUserService, claimsPrincipalHelper)
		{
			_logger = logger;
		}

        public async Task<IActionResult> OnGet()
        {
			_logger.LogMethodEntered();

			try
			{
				await SetTrustAddress();
				LoadPageComponents();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public IActionResult OnPost()
		{
			_logger.LogMethodEntered();

			LoadPageComponents();

			if (CaseDivision.SelectedId == null)
			{
				ModelState.AddModelError($"{nameof(CaseDivision)}.{CaseDivision.DisplayName}", "Select division");
				return Page();
			}

			if (CaseDivision.SelectedSubId == null) 
			{
				var subError = CaseDivision.SelectedId == (int)Division.SFSO ? "territory" : "region";

				ModelState.AddModelError($"{nameof(CaseDivision)}.{CaseDivision.DisplayName}", $"Select {subError}");
			}

			return Page();
		}

		private void LoadPageComponents()
		{
			CaseDivision = BuildCaseDivisionComponent(CaseDivision?.SelectedId, CaseDivision?.SelectedSubId);
		}

		private RadioButtonsUiComponent BuildCaseDivisionComponent(int? selectedId = null, int? selectedSubId = null)
		{
			var radioItems = new List<SimpleRadioItem>()
			{
				new SimpleRadioItem(Division.SFSO.GetDescription(), (int)Division.SFSO)
				{
					SubRadioItems = new List<SubRadioItem>()
					{
						new SubRadioItem(Territory.North_And_Utc__North_West.Description(), (int)Territory.North_And_Utc__North_West) { TestId = Territory.North_And_Utc__North_West.Description() },
						new SubRadioItem(Territory.National_Operations.Description(), (int)Territory.National_Operations) { TestId = Territory.National_Operations.Description() },
					}
				},
				new SimpleRadioItem(Division.RegionsGroup.GetDescription(), (int)Division.RegionsGroup)
				{
					SubRadioItems = new List<SubRadioItem>()
					{
						new SubRadioItem(Region.NorthEngland.Description(), (int)Region.NorthEngland) { TestId = Region.NorthEngland.Description() },
						new SubRadioItem(Region.SouthEngland.Description(), (int)Region.SouthEngland) { TestId = Region.SouthEngland.Description() },
						new SubRadioItem(Region.WestEngland.Description(),(int) Region.WestEngland) { TestId = Region.WestEngland.Description() },
						new SubRadioItem(Region.EastEngland.Description(),(int) Region.EastEngland) { TestId = Region.EastEngland.Description() },
					}
				}
			};

			return new(ElementRootId: "case-division", nameof(CaseDivision), "Who is managing this case?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				SelectedSubId = selectedSubId,
				DisplayName = "case division",
				SubItemDisplayName = "territory"
			};
		}

	}
}
