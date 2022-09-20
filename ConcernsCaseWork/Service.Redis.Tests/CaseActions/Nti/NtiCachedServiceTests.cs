using ConcernsCasework.Service.Nti;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Nti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Tests.CaseActions.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class NtiCachedServiceTests
	{
		private const string High_Level_Cache_Key = "NoticeToImprove";

		[Test]
		public async Task CreateNti_SaveToDBAndUpdateCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var newNtiDto = GetNewNti();
			var createdNtiDto = GetCreatedNti();

			mockNtiService.Setup(s => s.CreateNtiAsync(newNtiDto))
				.Returns(Task.FromResult(createdNtiDto));

			var sut = new NtiCachedService(mockCacheProvider.Object,
					mockNtiService.Object, mockLogger.Object);

			// act
			await sut.CreateNtiAsync(newNtiDto);

			// assert
			mockNtiService.Verify(s => s.CreateNtiAsync(newNtiDto), Times.Once);
			mockCacheProvider.Verify(c =>
				c.SetCache(CreateCacheKeyForNti(createdNtiDto.Id), createdNtiDto, It.IsAny<DistributedCacheEntryOptions>()),
			Times.Once);
			mockCacheProvider.Verify(c => c.ClearCache(CreateCacheKeyForNtisForCase(createdNtiDto.CaseUrn)), Times.Once);
		}

		[Test]
		public async Task GetNti_WhenFoundInCache_ReturnFromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var ntiFromDB = GetCreatedNti();

			mockCacheProvider.Setup(c => c.GetFromCache<NtiDto>(CreateCacheKeyForNti(ntiFromDB.Id)))
				.Returns(Task.FromResult(ntiFromDB));

			var sut = new NtiCachedService(mockCacheProvider.Object,
					mockNtiService.Object, mockLogger.Object);

			// act
			await sut.GetNtiAsync(ntiFromDB.Id);

			// assert
			mockNtiService.Verify(s => s.GetNtiAsync(ntiFromDB.Id), Times.Never);
			mockCacheProvider.Verify(c => c.GetFromCache<NtiDto>(CreateCacheKeyForNti(ntiFromDB.Id)), Times.Once);
		}

		[Test]
		public async Task GetNti_WhenNotFoundInCache_ReturnFromDb()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var ntiFromDB = GetCreatedNti();

			mockCacheProvider.Setup(c => c.GetFromCache<NtiDto>(CreateCacheKeyForNti(ntiFromDB.Id)))
				.Returns(Task.FromResult((NtiDto)null));

			mockNtiService.Setup(s => s.GetNtiAsync(ntiFromDB.Id))
				.Returns(Task.FromResult(ntiFromDB));

			var sut = new NtiCachedService(mockCacheProvider.Object,
					mockNtiService.Object, mockLogger.Object);

			// act
			await sut.GetNtiAsync(ntiFromDB.Id);

			// assert
			mockCacheProvider.Verify(c => c.GetFromCache<NtiDto>(CreateCacheKeyForNti(ntiFromDB.Id)), Times.Once);
			mockNtiService.Verify(s => s.GetNtiAsync(ntiFromDB.Id), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(CreateCacheKeyForNti(ntiFromDB.Id), ntiFromDB, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}


		[Test]
		public async Task GetNtisForCase_WhenFoundInCache_ReturnFromCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var ntisFromDB = GetPopulatedCollection();

			long caseUrn = ntisFromDB.First().CaseUrn;
			var cacheKey = CreateCacheKeyForNtisForCase(caseUrn);

			mockCacheProvider.Setup(c => c.GetFromCache<ICollection<NtiDto>>(cacheKey))
				.Returns(Task.FromResult(ntisFromDB));

			var sut = new NtiCachedService(mockCacheProvider.Object, mockNtiService.Object, 
				mockLogger.Object);

			// act
			await sut.GetNtisForCaseAsync(caseUrn);

			// assert
			mockNtiService.Verify(s => s.GetNtisForCaseAsync(caseUrn), Times.Never);
			mockCacheProvider.Verify(c => c.GetFromCache<ICollection<NtiDto>>(cacheKey), Times.Once);
		}

		[Test]
		public async Task GetNtisForCase_WhenNotFoundInCache_ReturnFromDB()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var ntisFromDB = GetPopulatedCollection();
			long caseUrn = ntisFromDB.First().CaseUrn;
			var cacheKey = CreateCacheKeyForNtisForCase(caseUrn);

			mockCacheProvider.Setup(c => c.GetFromCache<ICollection<NtiDto>>(cacheKey))
				.Returns(Task.FromResult(GetEmptyCollection()));
			mockNtiService.Setup(db => db.GetNtisForCaseAsync(caseUrn))
				.Returns(Task.FromResult(ntisFromDB));

			var sut = new NtiCachedService(mockCacheProvider.Object, mockNtiService.Object,
				mockLogger.Object);

			// act
			await sut.GetNtisForCaseAsync(caseUrn);

			// assert
			mockNtiService.Verify(s => s.GetNtisForCaseAsync(caseUrn), Times.Once);
			mockCacheProvider.Verify(c => c.GetFromCache<ICollection<NtiDto>>(cacheKey), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(cacheKey, ntisFromDB, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
		}

		[Test]
		public async Task PatchNti_UpdatesDBAndCache()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			var mockNtiService = new Mock<INtiService>();
			var mockLogger = new Mock<ILogger<NtiCachedService>>();

			var nti = GetCreatedNti();
			var cacheKey = CreateCacheKeyForNti(nti.Id);

			mockNtiService.Setup(db => db.PatchNtiAsync(nti))
				.Returns(Task.FromResult(nti));

			var sut = new NtiCachedService(mockCacheProvider.Object, mockNtiService.Object,
				mockLogger.Object);

			// act
			await sut.PatchNtiAsync(nti);

			// assert
			mockNtiService.Verify(db => db.PatchNtiAsync(nti), Times.Once);
			mockCacheProvider.Verify(c => c.SetCache(cacheKey, nti, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
			mockCacheProvider.Verify(c => c.ClearCache(CreateCacheKeyForNtisForCase(nti.CaseUrn)), Times.Once);
		}

		private NtiDto GetNewNti()
		{
			return new NtiDto
			{
				CaseUrn = 1234L,
				CreatedAt = DateTime.Now,
				Notes = "Test notes",
				StatusId = 1
			};
		}

		private NtiDto GetCreatedNti()
		{
			var nti = GetNewNti();
			nti.Id = 1234L;

			return nti;
		}

		private string CreateCacheKeyForNti(long ntiId)
		{
			return $"{High_Level_Cache_Key}:Nti:Id:{ntiId}";
		}

		private string CreateCacheKeyForNtisForCase(long caseUrn)
		{
			return $"{High_Level_Cache_Key}:NtiForCase:CaseUrn:{caseUrn}";
		}

		private ICollection<NtiDto> GetEmptyCollection()
		{
			return Array.Empty<NtiDto>();
		}

		private ICollection<NtiDto> GetPopulatedCollection()
		{
			long caseUrn = 3452L;
			return Enumerable.Range(1, 5).Select(i => new NtiDto { Id = i, CreatedAt = DateTime.Now, CaseUrn = caseUrn }).ToArray();
		}
	}
}