using System;
using System.Text.Json.Serialization;

namespace Service.Redis.Security
{
	[Serializable]
	public sealed class Claims
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }
        
		[JsonPropertyName("email")]
		public string Email { get; set; }
	}
}