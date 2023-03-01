namespace ConcernsCaseWork.Service.AzureAd
{
	public class AzureAdGraphSettings
	{
		public string ClientId { get; init; }
		public string ClientSecret { get; init; }
		public string TenantId { get; init; }

		public string[] AzureAdgroups { get; init; }
	}
}
