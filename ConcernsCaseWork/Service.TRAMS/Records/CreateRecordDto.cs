﻿using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Records
{
	public sealed class CreateRecordDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("reviewAt")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonProperty("closedAt")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("caseUrn")]
		public long CaseUrn { get; }
		
		[JsonProperty("typeUrn")]
		public long TypeUrn { get; }

		[JsonProperty("ratingUrn")]
		public long RatingUrn { get; }
		
		[JsonProperty("statusUrn")]
		public long StatusUrn { get; }
		
		[JsonProperty("meansOfReferralUrn")]
		public long MeansOfReferralUrn { get; }
		
		[JsonConstructor]
		public CreateRecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, long caseUrn, long typeUrn, 
			long ratingUrn, long statusUrn, long meansOfReferralUrn) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, StatusUrn, MeansOfReferralUrn) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, statusUrn, meansOfReferralUrn);
	}
}