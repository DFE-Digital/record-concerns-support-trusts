using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Mappers;

public static class TrustFinancialForecastMapping
{
	public static ActionSummaryModel ToActionSummary(this TrustFinancialForecastResponse model)
	{
		var result = new ActionSummaryModel()
		{
			ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
			Name = "Trust Financial Forecast (TFF)",
			OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
			RelativeUrl = $"/case/{model.CaseUrn}/management/action/trustfinancialforecast/{model.TrustFinancialForecastId}",
			StatusName = (model.ClosedAt.HasValue) ? "Completed" : "In progress",
			RawOpenedDate = model.CreatedAt,
			RawClosedDate = model.ClosedAt
		};

		return result;
	}
	
	public static TrustFinancialForecastSummaryModel ToTrustFinancialForecastSummaryModel(this TrustFinancialForecastResponse response)
	{
		var result = new TrustFinancialForecastSummaryModel()
		{
			Id = response.TrustFinancialForecastId,
			CaseUrn = response.CaseUrn,
			CreatedAt = response.CreatedAt.DateTime,
			UpdatedAt = response.UpdatedAt.DateTime,
			ClosedAt = response.ClosedAt?.DateTime
		};

		return result;
	}

	public static ViewTrustFinancialForecastModel ToViewModel(TrustFinancialForecastResponse response, GetCasePermissionsResponse permissionsResponse)
	{
		var isClosed = response.ClosedAt.HasValue;

		var result = new ViewTrustFinancialForecastModel()
		{
			IsClosed = response.ClosedAt.HasValue,
			IsEditable = !isClosed && permissionsResponse.HasEditPermissions(),
			SRMAOfferedAfterTFF = response.SRMAOfferedAfterTFF?.Description(),
			ForecastingToolRanAt = response.ForecastingToolRanAt?.Description(),
			WasTrustResponseSatisfactory = response.WasTrustResponseSatisfactory?.Description(),
			Notes = response.Notes,
			DateOpened = DateTimeHelper.ParseToDisplayDate(response.CreatedAt),
			DateClosed = DateTimeHelper.ParseToDisplayDate(response.ClosedAt),
			SFSOInitialReviewHappenedAt = DateTimeHelper.ParseToDisplayDate(response.SFSOInitialReviewHappenedAt),
			TrustRespondedAt = DateTimeHelper.ParseToDisplayDate(response.TrustRespondedAt)
		};

		return result;
	}
}