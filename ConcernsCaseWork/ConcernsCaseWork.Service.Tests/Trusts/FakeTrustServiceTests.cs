using ConcernsCaseWork.Service.Configuration;
using ConcernsCaseWork.Service.Trusts;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace ConcernsCaseWork.Service.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class FakeTrustServiceTests
	{
		private FakeTrustService _service = new(BuildFakeTrustOptions());

		[Test]
		public void GetTrustsByPagination_When_TrustExists_Returns_FakeTrust()
		{
			var result = _service.GetTrustsByPagination("Fake Trust 1");

			result.Trusts.Should().HaveCount(1);

			var trust = result.Trusts[0];

			trust.UkPrn.Should().Be("123");
			trust.GroupName.Should().Be("Fake Trust 1");
			trust.CompaniesHouseNumber.Should().Be("456");
		}

		[Test]
		public void GetTrustsByPagination_When_TrustDoesNotExist_Returns_FakeTrust()
		{
			var result = _service.GetTrustsByPagination("NotExist");

			result.Should().BeNull();
		}

		[Test]
		public void GetTrustByUkPrn_When_TrustExists_Returns_FakeTrust()
		{
			var result = _service.GetTrustByUkPrn("123");

			result.GiasData.UkPrn.Should().Be("123");
			result.GiasData.GroupName.Should().Be("Fake Trust 1");
			result.GiasData.CompaniesHouseNumber.Should().Be("456");
			result.GiasData.GroupContactAddress.Should().NotBeNull();
		}

		[Test]
		public void GetTrustByUkPrn_When_TrustDoesNotExist_Returns_Null()
		{
			var result = _service.GetTrustByUkPrn("NotExist");

			result.Should().BeNull();
		}

		private static IOptions<FakeTrustOptions> BuildFakeTrustOptions()
		{
			var trustOptions = new FakeTrustOptions
			{
				Trusts = new List<Trust>
				{
					new Trust()
					{
						UkPrn = "123",
						CompaniesHouseNumber = "456",
						Name = "Fake Trust 1"
					}
				}
			};

			var result = new Mock<IOptions<FakeTrustOptions>>();
			result.Setup(m => m.Value).Returns(trustOptions);

			return result.Object;
		}
	}
}
