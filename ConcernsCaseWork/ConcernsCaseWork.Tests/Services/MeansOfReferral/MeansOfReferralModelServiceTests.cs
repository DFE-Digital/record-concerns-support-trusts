using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Redis.MeansOfReferral;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.MeansOfReferral
{
	[Parallelizable(ParallelScope.All)]
	public class MeansOfReferralModelServiceTests
	{
		[Test]
		public async Task WhenGetMeansOfReferrals_ReturnsMeansOfReferrals()
		{
			// arrange
			var mockCachedService = new Mock<IMeansOfReferralCachedService>();
			var mockLogger = new Mock<ILogger<MeansOfReferralModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			
			var dtos = MeansOfReferralFactory.BuildListMeansOfReferralDto();

			mockCachedService.Setup(t => t.GetMeansOfReferralsAsync()).ReturnsAsync(dtos);
			
			var service = new MeansOfReferralModelService(mockCachedService.Object, mapper, mockLogger.Object);

			// act
			var result = await service.GetMeansOfReferrals();
			
			// assert
			Assert.That(result, Is.Not.Null);
			Assert.AreEqual(2, result.Count);
			
			Assert.AreEqual("Internal", result.First().Name);
			Assert.AreEqual("Some description 1", result.First().Description);
			Assert.AreEqual(1, result.First().Id);
			
			Assert.AreEqual("External", result.Last().Name);
			Assert.AreEqual("Some description 2", result.Last().Description);
			Assert.AreEqual(2, result.Last().Id);
		}
	}
}