namespace ConcernsCaseWork.Service.Trusts;

public record TrustSummaryDto(string UkPrn, string TrustName)
{
	public virtual string UkPrn { get; init; } = UkPrn;
	public virtual string TrustName { get; init; } = TrustName;
}