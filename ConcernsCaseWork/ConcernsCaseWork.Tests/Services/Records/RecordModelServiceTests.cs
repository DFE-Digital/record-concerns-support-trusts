using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Services.Records;
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

			var recordsDto = RecordFactory.BuildListRecordDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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
		}

		[Test]
		public async Task WhenGetRecordModelByUrn_When_Urn_IsUknown_Returns_RecordModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();
			var uknownRecordId = 0;

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);

			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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
		}

		[Test]
		public void WhenGetRecordModelByUrn_EmptyRecords_ThrowsException()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(Array.Empty<RecordDto>());
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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

			var recordsDto = RecordFactory.BuildListRecordDto();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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
		}
		
		[Test]
		public async Task WhenPostRecordByCaseUrn_ReturnTask()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();

			var recordDto = RecordFactory.BuildRecordDto();
			var createRecordModel = RecordFactory.BuildCreateRecordModel();
			
			mockRecordCacheService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>()))
				.ReturnsAsync(recordDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
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
		}

		[Test]
		public async Task WhenPatchRecordStatus_ReturnsTask()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var recordsDto = RecordFactory.BuildListRecordDto();

			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockLogger.Object);

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsDto);

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

			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object,
				mockLogger.Object);

			Assert.ThrowsAsync<ArgumentNullException>(() => recordModelService.PatchRecordStatus(RecordFactory.BuildPatchRecordModel()));

			// assert
			mockRecordCacheService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Never);
		}
	}
}