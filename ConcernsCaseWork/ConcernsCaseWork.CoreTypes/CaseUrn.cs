using Ardalis.GuardClauses;

namespace ConcernsCaseWork.CoreTypes;

public record struct CaseUrn
{
	public CaseUrn(long caseUrnValue)
	{
		Value = Guard.Against.NegativeOrZero(caseUrnValue);
	}

	public long Value { get; }

	public static explicit operator CaseUrn(long caseUrnValue) => new(caseUrnValue);
	public static implicit operator long(CaseUrn caseUrn) => caseUrn.Value;
}