using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Tests.CaseActions.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class NtiConditionsCachedServiceTests
	{
		private const string CacheKey = "Nti.NoticeToImprove.Conditions";

		[Test]
		public async Task GetAllConditionsAsync_WhenNotFoundInCache_FetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<NtiConditionsCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiConditionDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetEmptyCollection()));

			var sut = new NtiConditionsCachedService(mockCacheProvider.Object,
					mockNtiConditionsService.Object, mockLogger.Object);

			// act
			await sut.GetAllConditionsAsync();

			// assert
			mockNtiConditionsService.Verify(cs => cs.GetAllConditionsAsync(), Times.Once);
		}

		[Test]
		public async Task GetAllConditionsAsync_WhenFoundInCache_DoesntFetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<NtiConditionsCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiConditionDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetPopulatedCollection()));


			var sut = new NtiConditionsCachedService(mockCacheProvider.Object,
					mockNtiConditionsService.Object, mockLogger.Object);

			// act
			await sut.GetAllConditionsAsync();

			// assert
			mockNtiConditionsService.Verify(cs => cs.GetAllConditionsAsync(), Times.Never);
		}

		private ICollection<NtiConditionDto> GetEmptyCollection()
		{
			return Array.Empty<NtiConditionDto>();
		}

		private ICollection<NtiConditionDto> GetPopulatedCollection()
		{
			return Enumerable.Range(1, 5).Select(i => new NtiConditionDto { Id = i, Name = i.ToString() }).ToArray();
		}
	}
}