using AutoFixture;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories;

public class TrustFinancialForecastFactoryTests
{
	private readonly IFixture _fixture = new Fixture();

	[Fact]
	public void ToResponseModel_MapsResponseModelCorrectly()
	{
		// arrange
		var trustFinancialForecast = _fixture.Create<TrustFinancialForecast>();

		// act
		var result = trustFinancialForecast.ToResponseModel();

		// assert
		result.Should().BeEquivalentTo(trustFinancialForecast, options => options.Excluding(x => x.Id));
		result.TrustFinancialForecastId.Should().Be(trustFinancialForecast.Id);
	}
}