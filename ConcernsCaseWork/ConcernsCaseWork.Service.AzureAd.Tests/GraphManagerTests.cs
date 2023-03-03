using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace ConcernsCaseWork.Service.AzureAd.Tests;

public class GraphManagerTests
{
	[Fact]
	public void GraphManager_Implements_IGraphManager()
	{
		typeof(GraphManager).Should().Implement<IGraphManager>();
	}
}