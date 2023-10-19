using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast
{
	public record DeleteTrustFinancialForecastRequest : GetTrustFinancialForecastsForCaseRequest
	{
		public int TrustFinancialForecastId { get; init; }

		public override bool IsValid() => base.IsValid()
										  && TrustFinancialForecastId > 0;
	}
}
