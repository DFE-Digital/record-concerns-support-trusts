using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models;

public class TrustFinancialForecast
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set;  }
	public int CaseUrn { get; set; }
	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }
	public DateTimeOffset? SFSOInitialReviewHappenedAt { get; set; }
	public DateTimeOffset? TrustRespondedAt { get; set; }
	public string Notes { get; set; }
}
