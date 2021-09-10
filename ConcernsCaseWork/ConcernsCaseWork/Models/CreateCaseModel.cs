﻿using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	public sealed class CreateCaseModel
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdateAt { get; set; }

		public DateTimeOffset ReviewAt { get; set; }

		public DateTimeOffset ClosedAt { get; set; }

		public string CreatedBy { get; set; }

		public string Description { get; set; }
		
		public string TrustUkPrn { get; set; }
		
		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; }

		public string CurrentStatus { get; set; }

		public string NextSteps { get; set; }

		public string ResolutionStrategy { get; set; }
		
		public BigInteger Status { get; set; }
		
		public string RecordType { get; set; }
		
		public string RecordSubType { get; set; }
		
		public string RagRating { get; set; }
	}
}