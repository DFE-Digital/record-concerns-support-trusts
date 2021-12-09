using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Records;
using Service.TRAMS.Records;
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
			var mockRecordCacheService = new Mock<IRecordCachedService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var recordsDto = RecordFactory.BuildListRecordDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			
			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, mapper, mockLogger.Object);
			var recordsModel = await recordModelService.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>());

			// assert
			Assert.That(recordsModel, Is.Not.Null);
			Assert.That(recordsModel.Count, Is.EqualTo(recordsDto.Count));
		}
		
		[Test]
		public async Task WhenGetRecordModelByUrn_Returns_RecordModel()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordCachedService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			
			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, mapper, mockLogger.Object);
			var recordModel = await recordModelService.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), recordDto.Urn);

			// assert
			Assert.That(recordModel, Is.Not.Null);
			Assert.That(recordModel.Description, Is.EqualTo(recordDto.Description));
			Assert.That(recordModel.Name, Is.EqualTo(recordDto.Name));
			Assert.That(recordModel.Reason, Is.EqualTo(recordDto.Reason));
			Assert.That(recordModel.Urn, Is.EqualTo(recordDto.Urn));
			Assert.That(recordModel.CaseUrn, Is.EqualTo(recordDto.CaseUrn));
			Assert.That(recordModel.ClosedAt, Is.EqualTo(recordDto.ClosedAt));
			Assert.That(recordModel.CreatedAt, Is.EqualTo(recordDto.CreatedAt));
			Assert.That(recordModel.RatingUrn, Is.EqualTo(recordDto.RatingUrn));
			Assert.That(recordModel.ReviewAt, Is.EqualTo(recordDto.ReviewAt));
			Assert.That(recordModel.StatusUrn, Is.EqualTo(recordDto.StatusUrn));
			Assert.That(recordModel.TypeUrn, Is.EqualTo(recordDto.TypeUrn));
			Assert.That(recordModel.UpdatedAt, Is.EqualTo(recordDto.UpdatedAt));
		}
		
		[Test]
		public void WhenGetRecordModelByUrn_EmptyRecords_ThrowsException()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordCachedService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(Array.Empty<RecordDto>());
			
			// act
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, mapper, mockLogger.Object);
			Assert.ThrowsAsync<Exception>(() => recordModelService.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()));
		}
	}
}