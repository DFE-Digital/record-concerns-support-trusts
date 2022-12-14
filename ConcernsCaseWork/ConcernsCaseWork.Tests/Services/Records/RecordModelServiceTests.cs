using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Records
{
	[Parallelizable(ParallelScope.All)]
	public class RecordModelServiceTests
	{
		[Test]
		public async Task WhenGetRecordsModelByCaseUrn_Returns_RecordsModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var statusesDto = StatusFactory.BuildListStatusDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			var recordsModel = await recordModelService.GetRecordsModelByCaseUrn(It.IsAny<long>());

			// assert
			Assert.That(recordsModel, Is.Not.Null);
			Assert.That(recordsModel.Count, Is.EqualTo(recordsDto.Count));
		}
		
		[Test]
		public async Task WhenGetRecordModelByUrn_Returns_RecordModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var statusesDto = StatusFactory.BuildListStatusDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			var recordModel = await recordModelService.GetRecordModelById(It.IsAny<long>(), recordDto.Id);

			// assert
			Assert.That(recordModel, Is.Not.Null);
			Assert.That(recordModel.Id, Is.EqualTo(recordDto.Id));
			Assert.That(recordModel.CaseUrn, Is.EqualTo(recordDto.CaseUrn));
			Assert.That(recordModel.RatingId, Is.EqualTo(recordDto.RatingId));
			Assert.That(recordModel.StatusId, Is.EqualTo(recordDto.StatusId));
			Assert.That(recordModel.TypeId, Is.EqualTo(recordDto.TypeId));

			var firstRatingModel = ratingsDto.First();
			var firstTypeModel = typesDto.First();
			var firstStatusModel = statusesDto.First();
			
			Assert.NotNull(firstRatingModel);
			Assert.NotNull(recordModel.RatingModel);
			Assert.That(recordModel.RatingModel.Name, Is.EqualTo(firstRatingModel.Name));
			Assert.That(recordModel.RatingModel.Id, Is.EqualTo(firstRatingModel.Id));

			Assert.NotNull(firstTypeModel);
			Assert.NotNull(recordModel.TypeModel);
			Assert.That(recordModel.TypeModel.Type, Is.EqualTo(firstTypeModel.Name));
			Assert.That(recordModel.TypeModel.SubType, Is.EqualTo(firstTypeModel.Description));

			Assert.NotNull(firstStatusModel);
			Assert.NotNull(recordModel.StatusModel);
			Assert.That(recordModel.StatusModel.Name, Is.EqualTo(firstStatusModel.Name));
			Assert.That(recordModel.StatusModel.Id, Is.EqualTo(firstStatusModel.Id));
		}

		[Test]
		public async Task WhenGetRecordModelByUrn_When_Urn_IsUknown_Returns_RecordModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			var statusesDto = StatusFactory.BuildListStatusDto();
			var uknownRecordId = 0;

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);

			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object,
				mockTypeModelService.Object,
				mockLogger.Object);

			// act
			var recordModel = await recordModelService.GetRecordModelById(It.IsAny<long>(), uknownRecordId);

			// assert
			Assert.That(recordModel, Is.Not.Null);
			Assert.That(recordModel.Id, Is.EqualTo(recordDto.Id));
			Assert.That(recordModel.CaseUrn, Is.EqualTo(recordDto.CaseUrn));
			Assert.That(recordModel.RatingId, Is.EqualTo(recordDto.RatingId));
			Assert.That(recordModel.StatusId, Is.EqualTo(recordDto.StatusId));
			Assert.That(recordModel.TypeId, Is.EqualTo(recordDto.TypeId));

			var firstRatingModel = ratingsDto.First();
			var firstTypeModel = typesDto.First();
			var firstStatusModel = statusesDto.First();

			Assert.NotNull(firstRatingModel);
			Assert.NotNull(recordModel.RatingModel);
			Assert.That(recordModel.RatingModel.Name, Is.EqualTo(firstRatingModel.Name));
			Assert.That(recordModel.RatingModel.Id, Is.EqualTo(firstRatingModel.Id));

			Assert.NotNull(firstTypeModel);
			Assert.NotNull(recordModel.TypeModel);
			Assert.That(recordModel.TypeModel.Type, Is.EqualTo(firstTypeModel.Name));
			Assert.That(recordModel.TypeModel.SubType, Is.EqualTo(firstTypeModel.Description));

			Assert.NotNull(firstStatusModel);
			Assert.NotNull(recordModel.StatusModel);
			Assert.That(recordModel.StatusModel.Name, Is.EqualTo(firstStatusModel.Name));
			Assert.That(recordModel.StatusModel.Id, Is.EqualTo(firstStatusModel.Id));
		}

		[Test]
		public void WhenGetRecordModelByUrn_EmptyRecords_ThrowsException()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(Array.Empty<RecordDto>());
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			Assert.ThrowsAsync<Exception>(() => recordModelService.GetRecordModelById(It.IsAny<long>(), It.IsAny<long>()));
		}

		[Test]
		public async Task WhenGetCreateRecordsModelByCaseUrn_ReturnCreateRecordsModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			var createRecordsModel = await recordModelService.GetCreateRecordsModelByCaseUrn(It.IsAny<long>());

			// assert
			Assert.NotNull(createRecordsModel);
			Assert.That(createRecordsModel.Count, Is.EqualTo(recordsDto.Count));

			for (var index = 0; index < createRecordsModel.Count; ++index)
			{
				var actualRecordModel = createRecordsModel.ElementAt(index);
				var expectedRecordDto = recordsDto.ElementAt(index);
				
				Assert.That(actualRecordModel.TypeId, Is.EqualTo(expectedRecordDto.TypeId));
				Assert.That(actualRecordModel.CaseUrn, Is.EqualTo(expectedRecordDto.CaseUrn));
				Assert.That(actualRecordModel.RatingId, Is.EqualTo(expectedRecordDto.RatingId));
			}
			
			mockRecordCacheService.Verify(r => r.GetRecordsByCaseUrn(It.IsAny<long>()), Times.Once);
			mockTypeModelService.Verify(t => t.GetTypes(), Times.Once);
			mockRatingModelService.Verify(t => t.GetRatings(), Times.Once);
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnTask()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var recordDto = RecordFactory.BuildRecordDto();
			var statusDto = StatusFactory.BuildStatusDto("live", 1);
			var createRecordModel = RecordFactory.BuildCreateRecordModel();
			
			mockRecordCacheService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync(recordDto);
			mockStatusCachedService.Setup(t => t.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(statusDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			var actualRecordDto = await recordModelService.PostRecordByCaseUrn(createRecordModel);
			
			// assert
			Assert.NotNull(actualRecordDto);
			Assert.That(actualRecordDto.TypeId, Is.EqualTo(recordDto.TypeId));
			Assert.That(actualRecordDto.CaseUrn, Is.EqualTo(recordDto.CaseUrn));
			Assert.That(actualRecordDto.RatingId, Is.EqualTo(recordDto.RatingId));
			Assert.That(actualRecordDto.StatusId, Is.EqualTo(recordDto.StatusId));
			
			mockRecordCacheService.Verify(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()), Times.Once);
			mockStatusCachedService.Verify(t => t.GetStatusByName(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public async Task WhenPatchRecordStatus_ReturnsTask()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var recordsDto = RecordFactory.BuildListRecordDto();
			var statusesDto = StatusFactory.BuildListStatusDto();

			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object,
				mockTypeModelService.Object,
				mockLogger.Object);

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockStatusCachedService.Setup(s => s.GetStatuses())
				.ReturnsAsync(statusesDto);

			await recordModelService.PatchRecordStatus(RecordFactory.BuildPatchRecordModel());

			// assert
			mockRecordCacheService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Once);
		}

		[Test]
		public void WhenPatchRecordStatus_ThrowsException()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockStatusCachedService.Object,
				mockRatingModelService.Object,
				mockTypeModelService.Object,
				mockLogger.Object);

			Assert.ThrowsAsync<ArgumentNullException>(() => recordModelService.PatchRecordStatus(RecordFactory.BuildPatchRecordModel()));

			// assert
			mockRecordCacheService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Never);
		}
	}
}