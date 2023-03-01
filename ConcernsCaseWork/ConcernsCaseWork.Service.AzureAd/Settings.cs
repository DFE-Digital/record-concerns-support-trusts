// using System.Globalization;
// using Microsoft.Extensions.Configuration;
//
// namespace spike_ad_roles;
//
// public class Settings
// {
// 	public string? ClientId { get; init; }
// 	public string? ClientSecret { get; init; }
// 	public string? TenantId { get; init; }
// 	public string? GroupName { get; init; }
// 	
// 	public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";
// 	public string ApiUrl { get; set; } = "https://graph.microsoft.cosm/";
//
// 	public string Authority => string.Format(CultureInfo.InvariantCulture, Instance, TenantId);
// 	public IEnumerable<string> Scopes => new [] {$"{ApiUrl}.default"};
// 	
// 	public static Settings LoadSettings() => new ConfigurationBuilder()
// 		.AddUserSecrets<Settings>()
// 		.Build()
// 		.GetRequiredSection(nameof(Settings))
// 		.Get<Settings>();
// }