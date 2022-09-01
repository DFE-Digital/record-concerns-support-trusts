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
	public class NtiStatusCachedServiceTests
	{
		private const string CacheKey = "Nti.NoticeToImprove.Statuses";

		[Test]
		public async Task GetAllStatusesAsync_WhenNotFoundInCache_FetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiStatusesService = new Mock<INtiStatusesService>();
			var mockLogger = new Mock<ILogger<NtiStatusesCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiStatusDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetEmptyCollection()));


			var sut = new NtiStatusesCachedService(mockCacheProvider.Object,
					mockNtiStatusesService.Object, mockLogger.Object);

			// act
			await sut.GetAllStatusesAsync();

			// assert
			mockNtiStatusesService.Verify(ss => ss.GetNtiStatusesAsync(), Times.Once);
		}

		[Test]
		public async Task GetAllStatusesAsync_WhenFoundInCache_DoesntFetchFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiStatusesService = new Mock<INtiStatusesService>();
			var mockLogger = new Mock<ILogger<NtiStatusesCachedService>>();

			mockCacheProvider.Setup(p => p.GetFromCache<ICollection<NtiStatusDto>>(CacheKey))
				.Returns(() => Task.FromResult(GetPopulatedCollection()));


			var sut = new NtiStatusesCachedService(mockCacheProvider.Object,
					mockNtiStatusesService.Object, mockLogger.Object);

			// act
			await sut.GetAllStatusesAsync();

			// assert
			mockNtiStatusesService.Verify(ss => ss.GetNtiStatusesAsync(), Times.Never);
		}

		private ICollection<NtiStatusDto> GetEmptyCollection()
		{
			return Array.Empty<NtiStatusDto>();
		}

		private ICollection<NtiStatusDto> GetPopulatedCollection()
		{
			return Enumerable.Range(1, 5).Select(i => new NtiStatusDto { Id = i, Name = i.ToString() }).ToArray();
		}
	}
}