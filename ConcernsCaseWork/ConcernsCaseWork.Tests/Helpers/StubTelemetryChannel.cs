using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System;

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
		
		// public void Test()
		// {
		// 	using var operation = _telemetryClient.StartOperation<RequestTelemetry>("ItWorks");
		// }
	}
}