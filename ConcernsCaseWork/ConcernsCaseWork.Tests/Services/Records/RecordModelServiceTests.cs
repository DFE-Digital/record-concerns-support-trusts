using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Types;
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
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, 
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
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
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();

			var recordsDto = RecordFactory.BuildListRecordDto();
			var recordDto = recordsDto.First();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();

			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsDto);
			mockTypeModelService.Setup(t => t.GetTypes())
				.ReturnsAsync(typesDto);
			mockRatingModelService.Setup(t => t.GetRatings())
				.ReturnsAsync(ratingsDto);
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, 
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			var recordModel = await recordModelService.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), recordDto.Urn);

			// assert
			Assert.That(recordModel, Is.Not.Null);
			Assert.That(recordModel.Urn, Is.EqualTo(recordDto.Urn));
			Assert.That(recordModel.CaseUrn, Is.EqualTo(recordDto.CaseUrn));
			Assert.That(recordModel.RatingUrn, Is.EqualTo(recordDto.RatingUrn));
			Assert.That(recordModel.StatusUrn, Is.EqualTo(recordDto.StatusUrn));
			Assert.That(recordModel.TypeUrn, Is.EqualTo(recordDto.TypeUrn));

			var firstRatingModel = ratingsDto.First();
			var firstTypeModel = typesDto.First();
			
			Assert.NotNull(firstRatingModel);
			Assert.NotNull(recordModel.RatingModel);
			Assert.That(recordModel.RatingModel.Name, Is.EqualTo(firstRatingModel.Name));
			Assert.That(recordModel.RatingModel.Urn, Is.EqualTo(firstRatingModel.Urn));

			Assert.NotNull(firstTypeModel);
			Assert.NotNull(recordModel.TypeModel);
			Assert.That(recordModel.TypeModel.Type, Is.EqualTo(firstTypeModel.Name));
			Assert.That(recordModel.TypeModel.SubType, Is.EqualTo(firstTypeModel.Description));
		}
		
		[Test]
		public void WhenGetRecordModelByUrn_EmptyRecords_ThrowsException()
		{
			// arrange
			var mockRecordCacheService = new Mock<IRecordCachedService>();
			var mockLogger = new Mock<ILogger<RecordModelService>>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			
			mockRecordCacheService.Setup(r => r.GetRecordsByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(Array.Empty<RecordDto>());
			
			var recordModelService = new RecordModelService(mockRecordCacheService.Object, 
				mockRatingModelService.Object, 
				mockTypeModelService.Object, 
				mockLogger.Object);
			
			// act
			Assert.ThrowsAsync<Exception>(() => recordModelService.GetRecordModelByUrn(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()));
		}
	}
}