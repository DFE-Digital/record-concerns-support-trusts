using ConcernsCaseWork.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging.EventSource;
using System.Text.Json;

namespace ConcernsCaseWork.Helpers;

public static class AppInsightsHelper
{

	public static void LogEvent(TelemetryClient client, AppInsightsModel model)
	{
		var payload = JsonSerializer.Serialize(model);
		client.TrackEvent(payload);
	}
}