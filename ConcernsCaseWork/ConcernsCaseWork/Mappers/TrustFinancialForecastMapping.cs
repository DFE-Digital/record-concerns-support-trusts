using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Mappers;

public static class TrustFinancialForecastMapping
{
	public static ActionSummaryModel ToActionSummary(this TrustFinancialForecastResponse model)
	{
		var result = new ActionSummaryModel()
		{
			ClosedDate = model.ClosedAt?.ToDayMonthYear(),
			Name = "Trust Financial Forecast (TFF)",
			OpenedDate = model.CreatedAt.ToDayMonthYear(),
			RelativeUrl = $"/case/{model.CaseUrn}/management/action/trustfinancialforecast/{model.TrustFinancialForecastId}",
			StatusName = (model.ClosedAt.HasValue) ? "Completed" : "In progress",
			RawOpenedDate = model.CreatedAt,
			RawClosedDate = model.ClosedAt,
		};

		return result;
	}
	
	public static TrustFinancialForecastSummaryModel ToTrustFinancialForecastSummaryModel(this TrustFinancialForecastResponse response)
	{
		var result = new TrustFinancialForecastSummaryModel()
		{
			Id = 0,
			CaseUrn = response.CaseUrn,
			CreatedAt = response.CreatedAt.DateTime,
			UpdatedAt = response.UpdatedAt.DateTime,
			ClosedAt = response.ClosedAt?.DateTime,
		};

		return result;
	}
}