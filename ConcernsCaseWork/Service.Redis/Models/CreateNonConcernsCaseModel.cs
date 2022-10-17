using System;

namespace Service.Redis.Models;

[Serializable]
public sealed class CreateNonConcernsCaseModel
{
	public string TrustUkPrn { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public DateTimeOffset ClosedAt { get; set; }
	public string CreatedBy { get; set; }
	public long StatusUrn { get; set; }
}