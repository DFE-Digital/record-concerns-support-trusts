using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Nti;
using Service.TRAMS.Cases;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class NtiReasonsCachedServiceTests
	{
		private const string CacheKey = "Nti.NoticeToImprove.Reasons";

		[Test]
		public async Task GetAllReasonsAsync_WhenNotFoundInCache_FetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiReasonsService = new Mock<INtiReasonsService>();
			var mockLogger = new Mock<ILogger<NtiReasonsCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiReasonDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetEmptyCollection()));


			var sut = new NtiReasonsCachedService(mockCacheProvider.Object,
					mockNtiReasonsService.Object, mockLogger.Object);

			// act
			await sut.GetAllReasonsAsync();

			// assert
			mockNtiReasonsService.Verify(ss => ss.GetNtiReasonsAsync(), Times.Once);
		}

		[Test]
		public async Task GetAllStatusesAsync_WhenFoundInCache_DoesntFetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiReasonsService = new Mock<INtiReasonsService>();
			var mockLogger = new Mock<ILogger<NtiReasonsCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiReasonDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetPopulatedCollection()));


			var sut = new NtiReasonsCachedService(mockCacheProvider.Object,
					mockNtiReasonsService.Object, mockLogger.Object);

			// act
			await sut.GetAllReasonsAsync();

			// assert
			mockNtiReasonsService.Verify(ss => ss.GetNtiReasonsAsync(), Times.Never);
		}

		private ICollection<NtiReasonDto> GetEmptyCollection()
		{
			return Array.Empty<NtiReasonDto>();
		}

		private ICollection<NtiReasonDto> GetPopulatedCollection()
		{
			return Enumerable.Range(1, 5).Select(i => new NtiReasonDto { Id = i, Name = i.ToString() }).ToArray();
		}
	}
}