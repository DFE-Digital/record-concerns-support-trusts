using System.Text.Json.Serialization;

namespace Service.Redis.Models
{
	public sealed class UserClaims
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }
        
		[JsonPropertyName("email")]
		public string Email { get; set; }
	}
}