using System;
using System.Reflection;
using ConcernsCaseWork.API.StartupConfiguration;
using Xunit;

namespace ConcernsCaseWork.API.Tests.StartupConfiguration;

public class ApiConfigurationExtensionsTests
{
	[Theory]
	[InlineData("localhost", true)]
	[InlineData("LOCALHOST", true)]
	[InlineData("127.0.0.1", true)]
	[InlineData("example.com", false)]
	[InlineData("::1", false)]
	public void IsLocalHost_ReturnsExpectedResult(string host, bool expected)
	{
		var result = InvokePrivateStaticMethod<bool>("IsLocalHost", host);

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("https://localhost", true)]
	[InlineData("https://LOCALHOST", true)]
	[InlineData("https://127.0.0.1", true)]
	[InlineData("http://localhost", false)]
	[InlineData("https://example.com", false)]
	[InlineData("https://[::1]", false)]
	public void IsLocalHttpsEndpoint_ReturnsExpectedResult(string endpoint, bool expected)
	{
		var result = InvokePrivateStaticMethod<bool>("IsLocalHttpsEndpoint", new Uri(endpoint));

		Assert.Equal(expected, result);
	}

	private static T InvokePrivateStaticMethod<T>(string methodName, object parameter)
	{
		var method = typeof(ApiConfigurationExtensions).GetMethod(
			methodName,
			BindingFlags.NonPublic | BindingFlags.Static);

		Assert.NotNull(method);
		return Assert.IsType<T>(method.Invoke(null, new[] { parameter }));
	}
}
