using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.CoreTypes
{
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
}
