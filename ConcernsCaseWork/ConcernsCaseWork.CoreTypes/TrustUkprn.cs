using Ardalis.GuardClauses;

namespace ConcernsCaseWork.CoreTypes;

public class TrustUkprn
{
	public TrustUkprn(long caseUrnValue)
	{
		Guard.Against.NegativeOrZero(caseUrnValue);
		Guard.Against.OutOfRange(caseUrnValue, nameof(caseUrnValue), 10000000, 19999999);
		Value = Guard.Against.NegativeOrZero(caseUrnValue);
	}

	private long Value { get; }

	public static explicit operator TrustUkprn(long caseUrnValue) => new(caseUrnValue);
	public static implicit operator long(TrustUkprn caseUrn) => caseUrn.Value;
}