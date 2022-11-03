using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.MeansOfReferral;
using ConcernsCaseWork.Service.MeansOfReferral;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.MeansOfReferral
{
	[Parallelizable(ParallelScope.All)]
	public class MeansOfReferralCachedServiceTests
	{
		[Test]
		public async Task WhenGetMeansOfReferrals_ReturnsMeansOfReferrals_FromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockMeansOfReferralService = new Mock<IMeansOfReferralService>();
			var mockLogger = new Mock<ILogger<MeansOfReferralCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<MeansOfReferralDto>>(It.IsAny<string>())).
				ReturnsAsync(MeansOfReferralFactory.BuildListMeansOfReferralDto());
			
			var meansOfReferralCachedService = new MeansOfReferralCachedService(mockCacheProvider.Object, mockMeansOfReferralService.Object, mockLogger.Object);

			// act
			var meansOfReferralsDto = await meansOfReferralCachedService.GetMeansOfReferralsAsync();
			
			// assert
			Assert.That(meansOfReferralsDto, Is.Not.Null);
			Assert.That(meansOfReferralsDto.Count, Is.EqualTo(2));
			mockCacheProvider.Verify(c => c.GetFromCache<IList<MeansOfReferralDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<MeansOfReferralDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
			mockMeansOfReferralService.Verify(c => c.GetMeansOfReferrals(), Times.Never);
		}

		[Test]
		public async Task WhenGetMeansOfReferrals_ReturnsMeansOfReferrals_FromTramsApi()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockMeansOfReferralService = new Mock<IMeansOfReferralService>();
			var mockLogger = new Mock<ILogger<MeansOfReferralCachedService>>();

			mockCacheProvider.Setup(c => c.GetFromCache<IList<MeansOfReferralDto>>(It.IsAny<string>())).
				Returns(Task.FromResult<IList<MeansOfReferralDto>>(null));
			mockMeansOfReferralService.Setup(r => r.GetMeansOfReferrals()).ReturnsAsync(MeansOfReferralFactory.BuildListMeansOfReferralDto());
			
			var meansOfReferralCachedService = new MeansOfReferralCachedService(mockCacheProvider.Object, mockMeansOfReferralService.Object, mockLogger.Object);

			// act
			var meansOfReferralsDto = await meansOfReferralCachedService.GetMeansOfReferralsAsync();
			
			// assert
			Assert.That(meansOfReferralsDto, Is.Not.Null);
			Assert.That(meansOfReferralsDto.Count, Is.EqualTo(2));
			mockCacheProvider.Verify(c => c.GetFromCache<IList<MeansOfReferralDto>>(It.IsAny<string>()), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(It.IsAny<string>(), It.IsAny<IList<MeansOfReferralDto>>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockMeansOfReferralService.Verify(c => c.GetMeansOfReferrals(), Times.Once);
		}
	}
}