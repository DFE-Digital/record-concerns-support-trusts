using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.ModelServices
{
	[Parallelizable(ParallelScope.All)]
	public class NtiModelServiceTests
	{
		[Test]
		public async Task CreateNtiAsync_CreatesNewNtiThroughCachedService()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();
			mockModelService.Setup(c => c.CreateNtiAsync(It.IsAny<NtiDto>())).ReturnsAsync(ntiDto);

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.CreateNtiAsync(ntiModel);

			// assert
			mockModelService.Verify(ms => ms.CreateNtiAsync(It.IsAny<NtiDto>()), Times.Once);
		}

		[Test]
		public async Task GetNtiAsync_GetsNtiThroughCachedService()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();

			mockModelService.Setup(c => c.GetNtiAsync(It.IsAny<string>())).ReturnsAsync(ntiDto);

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.GetNtiAsync(Guid.NewGuid().ToString());

			// assert
			mockModelService.Verify(ms => ms.GetNtiAsync(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task GetNtiByIdAsync_GetsNtiThroughCachedService()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();

			mockModelService.Setup(c => c.GetNtiAsync(ntiModel.Id)).ReturnsAsync(ntiDto);

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.GetNtiByIdAsync(ntiModel.Id);

			// assert
			mockModelService.Verify(ms => ms.GetNtiAsync(ntiModel.Id), Times.Once);
		}

		[Test]
		public async Task GetNtisForCaseAsync_GetsNtisThroughCachedService()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();

			mockModelService.Setup(c => c.GetNtisForCaseAsync(ntiModel.CaseUrn)).ReturnsAsync(GetNtiDtoCollection());

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.GetNtisForCaseAsync(ntiModel.CaseUrn);

			// assert
			mockModelService.Verify(ms => ms.GetNtisForCaseAsync(ntiModel.CaseUrn), Times.Once);
		}

		[Test]
		public async Task PatchNtiAsync_PatchNtiThroughCachedService()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();

			mockModelService.Setup(c => c.PatchNtiAsync(It.IsAny<NtiDto>())).ReturnsAsync(ntiDto);

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.PatchNtiAsync(ntiModel);

			// assert
			mockModelService.Verify(ms => ms.PatchNtiAsync(It.IsAny<NtiDto>()), Times.Once);
		}

		[Test]
		public async Task StoreNtiAsync_StoreNtiToCache()
		{
			// arrange
			var ntiModel = CreateNtiModel();
			var ntiDto = NtiMappers.ToDBModel(ntiModel);
			var continuationId = Guid.NewGuid().ToString();

			var mockModelService = new Mock<INtiCachedService>();
			var mockStatusCachedService = new Mock<INtiStatusesCachedService>();

			var sut = new NtiModelService(mockModelService.Object, mockStatusCachedService.Object);

			// act
			await sut.StoreNtiAsync(ntiModel, continuationId);

			// assert
			mockModelService.Verify(ms => ms.SaveNtiAsync(It.IsAny<NtiDto>(), continuationId), Times.Once);
		}


		private NtiModel CreateNtiModel()
		{
			return new NtiModel
			{
				Id = 123L,
				CaseUrn = 789L,
				CreatedAt = DateTime.Now
			};
		}

		private ICollection<NtiDto> GetNtiDtoCollection()
		{
			return new NtiDto[]
			{
				NtiMappers.ToDBModel(CreateNtiModel())
			};
		}
	}
}