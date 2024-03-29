using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Utils.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast;

[BindProperties]
public class EditableTrustFinancialForecastPageModel : AbstractPageModel
{
	[BindProperty(SupportsGet = true, Name="Urn")] public int CaseUrn { get; set; }
	public OptionalDateTimeUiComponent SFSOInitialReviewHappenedAt { get; set; } = BuildSFSOInitialReviewHappenedAtComponent();
	public OptionalDateTimeUiComponent TrustRespondedAt { get; set; } = BuildTrustRespondedAtComponent();
	public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();
	public RadioButtonsUiComponent WasTrustResponseSatisfactory { get; set; } = BuildWasTrustResponseSatisfactoryComponent();
	public RadioButtonsUiComponent SRMAOfferedAfterTFF { get; set; } = BuildSRMAOfferedAfterTFFComponent();
	public RadioButtonsUiComponent ForecastingToolRanAt { get; set; } = BuildForecastingToolRanAtComponent();
	
	[BindProperty(SupportsGet = true)] public string ReferenceDocUrl { get; } = "https://educationgovuk.sharepoint.com/sites/lveefa00003/SitePages/Trust%20financial%20forecasting.aspx";
	
	protected void ResetPageComponentsOnValidationError()
	{
		ForecastingToolRanAt = BuildForecastingToolRanAtComponent(ForecastingToolRanAt.SelectedId);
		SRMAOfferedAfterTFF = BuildSRMAOfferedAfterTFFComponent(SRMAOfferedAfterTFF.SelectedId);
		WasTrustResponseSatisfactory = BuildWasTrustResponseSatisfactoryComponent(WasTrustResponseSatisfactory.SelectedId);

		SFSOInitialReviewHappenedAt = BuildSFSOInitialReviewHappenedAtComponent(SFSOInitialReviewHappenedAt.Date);
		TrustRespondedAt = BuildTrustRespondedAtComponent(TrustRespondedAt.Date);
		Notes = BuildNotesComponent(Notes.Text.StringContents);
	}
		
	private static RadioButtonsUiComponent BuildForecastingToolRanAtComponent(int? selectedId = null) 
		=> new(ElementRootId: "forecasting-tool-ran-at", Name: nameof(ForecastingToolRanAt), "When was the forecasting tool run?")
		{
			RadioItems = new SimpleRadioItem[]
			{
				new (API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSpring.Description(), (int)API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSpring),
				new (API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSummer.Description(),(int)API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.CurrentYearSummer),
				new (API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSpring.Description(),(int)API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSpring),
				new (API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSummer.Description(),(int)API.Contracts.TrustFinancialForecast.ForecastingToolRanAt.PreviousYearSummer)
			},
			SelectedId = selectedId
		};
		
	private static RadioButtonsUiComponent BuildSRMAOfferedAfterTFFComponent(int? selectedId = null) 
		=> new("srma-offered-after-tff",  nameof(SRMAOfferedAfterTFF), "SRMA offered after TFF?")
		{
			RadioItems = new SimpleRadioItem[]
			{
				new (API.Contracts.TrustFinancialForecast.SRMAOfferedAfterTFF.Yes.Description(), (int)API.Contracts.TrustFinancialForecast.SRMAOfferedAfterTFF.Yes),
				new (API.Contracts.TrustFinancialForecast.SRMAOfferedAfterTFF.No.Description(),(int)API.Contracts.TrustFinancialForecast.SRMAOfferedAfterTFF.No)
			},
			SelectedId = selectedId
		};
		
	private static RadioButtonsUiComponent BuildWasTrustResponseSatisfactoryComponent(int? selectedId = null) 
		=> new("trust-response-satisfactory", nameof(WasTrustResponseSatisfactory), "Was the trust response satisfactory?")
		{
			RadioItems = new SimpleRadioItem[]
			{
				new (API.Contracts.TrustFinancialForecast.WasTrustResponseSatisfactory.Satisfactory.Description(), (int)API.Contracts.TrustFinancialForecast.WasTrustResponseSatisfactory.Satisfactory),
				new (API.Contracts.TrustFinancialForecast.WasTrustResponseSatisfactory.NotSatisfactory.Description(),(int)API.Contracts.TrustFinancialForecast.WasTrustResponseSatisfactory.NotSatisfactory)
			},
			SelectedId = selectedId
		};
		
	private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("notes", nameof(Notes), "Notes")
		{
			SortOrder = 3,
			Text = new ValidateableString()
			{
				MaxLength = TrustFinancialForecastConstants.MaxNotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};
		
	private static OptionalDateTimeUiComponent BuildTrustRespondedAtComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new ("trust-responded-at", nameof(TrustRespondedAt), "When did the trust respond?")
		{
			Date = selectedDate,
			DisplayName = "Date trust responded",
			SortOrder = 2
		};
		
	private static OptionalDateTimeUiComponent BuildSFSOInitialReviewHappenedAtComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new("sfso-initial-review-happened-at", nameof(SFSOInitialReviewHappenedAt), "When did SFSO initial review happen?")
		{
			Date = selectedDate,
			DisplayName = "Date SFSO initial review happened",
			SortOrder = 1
		};
}