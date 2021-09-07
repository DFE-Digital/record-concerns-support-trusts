using ConcernsCaseWork.Models.Redis;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Services;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Services
{
	[Parallelizable(ParallelScope.All)]
	public class CasesCachedServiceTests
	{
		[Test]
		public async Task WhenCreateCaseDataCache_IsSuccessful()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var casesCachedService = new CachedService(mockCacheProvider.Object);
			var caseStateModel = new CaseStateModel
			{
				TrustUkPrn = "999999"
			};
			
			mockCacheProvider.Setup(c => c.CacheTimeToLive()).Returns(120);
			mockCacheProvider.Setup(c => c.GetFromCache<CaseStateModel>(It.IsAny<string>())).
				Returns(Task.FromResult(caseStateModel));

			// act
			await casesCachedService.StoreData("username", caseStateModel);
			var cachedCaseStateData = await casesCachedService.GetData<CaseStateModel>("username");

			// assert
			Assert.That(cachedCaseStateData, Is.Not.Null);
			Assert.That(cachedCaseStateData, Is.InstanceOf<CaseStateModel>());
			Assert.That(cachedCaseStateData.TrustUkPrn, Is.EqualTo(caseStateModel.TrustUkPrn));
		}
	}
}