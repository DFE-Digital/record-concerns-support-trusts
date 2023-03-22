using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Concurrent;

namespace ConcernsCaseWork.Tests.Helpers;

internal sealed class StubTelemetryChannel : ITelemetryChannel
{
	private readonly Action<ITelemetry> _onSend;
	public StubTelemetryChannel(Action<ITelemetry> onSend) => _onSend = onSend ?? throw new ArgumentNullException(nameof(onSend));
	public bool? DeveloperMode { get; set; }
	public string EndpointAddress { get; set; } = "";
	public void Dispose() { }
	public void Flush() { }
	public void Send(ITelemetry item) => _onSend(item);
	
	internal class ComponentUnderTest
	{
		private TelemetryClient _telemetryClient;
		public ComponentUnderTest(TelemetryClient telemetryClient) => _telemetryClient = telemetryClient;
		
		
	}
	
	
}

public static class MockTelemetry
{
	private static ConcurrentQueue<ITelemetry> TelemetryItems { get; } = new ConcurrentQueue<ITelemetry>();
	public static TelemetryClient CreateMockTelemetryClient()
	{
		var telemetryConfiguration = new TelemetryConfiguration
		{
			ConnectionString = "InstrumentationKey=" + Guid.NewGuid().ToString(),
			TelemetryChannel = new StubTelemetryChannel(TelemetryItems.Enqueue)
		};

			
		return new TelemetryClient(telemetryConfiguration);
	}
}