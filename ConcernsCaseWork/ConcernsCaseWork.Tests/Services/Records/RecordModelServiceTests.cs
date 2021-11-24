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