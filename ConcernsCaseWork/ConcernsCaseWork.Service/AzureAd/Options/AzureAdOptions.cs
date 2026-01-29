using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ConcernsCaseWork.Service.AzureAd.Options;

[ExcludeFromCodeCoverage(Justification = "This is just a class with properties, no logic")]
public class AzureAdOptions
{
	public Guid TenantId { get; set; }

	public string ClientSecret { get; set; }

	public Guid ClientId { get; set; }

	public string ApiUrl { get; set; } = "https://graph.microsoft.com/";

	public string Authority => string.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}", TenantId);

	public IEnumerable<string> Scopes => new[] { $"{ApiUrl}.default" };
}
