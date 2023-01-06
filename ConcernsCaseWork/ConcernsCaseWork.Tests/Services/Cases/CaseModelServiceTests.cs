using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseModelServiceTests
	{

		[Test]
		public async Task WhenPostCase_ReturnsCaseUrn()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			var recordDto = RecordFactory.BuildRecordDto();
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();

			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);
			mockCaseService.Setup(cs => cs.PostCase(It.IsAny<CreateCaseDto>())).ReturnsAsync(caseDto);
			mockRecordService.Setup(r => r.PostRecordByCaseUrn(It.IsAny<CreateRecordDto>())).ReturnsAsync(recordDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			var actualCaseUrn = await caseModelService.PostCase(createCaseModel);

			// assert
			Assert.That(actualCaseUrn, Is.Not.Zero);
			Assert.That(actualCaseUrn, Is.EqualTo(caseDto.Urn));
		}
		
		[Test]
		public void WhenPostCase_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			var statusLiveDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var statusMonitoringDto = StatusFactory.BuildStatusDto(StatusEnum.Monitoring.ToString(), 2);
			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			
			mockStatusCachedService.SetupSequence(s => s.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusLiveDto).ReturnsAsync(statusMonitoringDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			// assert
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PostCase(createCaseModel));
		}

		[Test]
		public async Task When_GetCaseByUrn_ReturnsCaseModel()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseDto);
			mockRecordService.Setup(r => r.GetRecordsByCaseUrn(caseDto.Urn))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			var actualCaseModel = await caseModelService.GetCaseByUrn(1);

			// assert
			Assert.IsAssignableFrom<CaseModel>(actualCaseModel);
			Assert.That(actualCaseModel, Is.Not.Null);
			Assert.That(actualCaseModel.CreatedAt, Is.EqualTo(caseDto.CreatedAt));
			Assert.That(actualCaseModel.UpdatedAt, Is.EqualTo(caseDto.UpdatedAt));
			Assert.That(actualCaseModel.ReviewAt, Is.EqualTo(caseDto.ReviewAt));
			Assert.That(actualCaseModel.ClosedAt, Is.EqualTo(caseDto.ClosedAt));
			Assert.That(actualCaseModel.CreatedBy, Is.EqualTo(caseDto.CreatedBy));
			Assert.That(actualCaseModel.Description, Is.EqualTo(caseDto.Description));
			Assert.That(actualCaseModel.CrmEnquiry, Is.EqualTo(caseDto.CrmEnquiry));
			Assert.That(actualCaseModel.TrustUkPrn, Is.EqualTo(caseDto.TrustUkPrn));
			Assert.That(actualCaseModel.ReasonAtReview, Is.EqualTo(caseDto.ReasonAtReview));
			Assert.That(actualCaseModel.DeEscalation, Is.EqualTo(caseDto.DeEscalation));
			Assert.That(actualCaseModel.Issue, Is.EqualTo(caseDto.Issue));
			Assert.That(actualCaseModel.CurrentStatus, Is.EqualTo(caseDto.CurrentStatus));
			Assert.That(actualCaseModel.NextSteps, Is.EqualTo(caseDto.NextSteps));
			Assert.That(actualCaseModel.CaseAim, Is.EqualTo(caseDto.CaseAim));
			Assert.That(actualCaseModel.DeEscalationPoint, Is.EqualTo(caseDto.DeEscalationPoint));
			Assert.That(actualCaseModel.DirectionOfTravel, Is.EqualTo(caseDto.DirectionOfTravel));
			Assert.That(actualCaseModel.Urn, Is.EqualTo(1));
			Assert.That(actualCaseModel.StatusId, Is.EqualTo(1));
			Assert.That(actualCaseModel.RatingId, Is.EqualTo(caseDto.RatingId));
			Assert.IsNull(actualCaseModel.RatingModel);
		}
		
		[Test]
		public void When_GetCaseByUrn_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockRecordService.Setup(r => r.GetRecordsByCaseUrn(caseDto.Urn))
				.ReturnsAsync(RecordFactory.BuildListRecordDto());

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.GetCaseByUrn(1));
		}
		
		[Test]
		public async Task WhenPatchCaseRating_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);
			
			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchCaseRating(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCaseRating_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCaseRating(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Never);
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchRecordRating_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var recordsDto = RecordFactory.BuildListRecordDto();

			mockRecordService.Setup(rs => rs.GetRecordsByCaseUrn(It.IsAny<long>())).ReturnsAsync(recordsDto);

			// act
					var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			await caseModelService.PatchRecordRating(RecordFactory.BuildPatchRecordModel());

			// assert
			mockRecordService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Once);
		}

		[Test]
		public void WhenPatchRecordRating_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			Assert.ThrowsAsync<ArgumentNullException>(() => caseModelService.PatchRecordRating(RecordFactory.BuildPatchRecordModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchClosure_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			mockStatusCachedService.Setup(t => t.GetStatusByName(It.IsAny<string>())).ReturnsAsync(statusDto);
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);
			mockCaseService.Setup(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()));
			
			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
			mockStatusCachedService.Verify(t => t.GetStatusByName(It.IsAny<string>()), Times.Once);
			mockCaseService.Verify(cs => cs.GetCaseByUrn(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchClosure_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchClosure(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockRecordService.Verify(r => r.PatchRecordById(It.IsAny<RecordDto>()), Times.Never);
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchDirectionOfTravel_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchDirectionOfTravel(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchDirectionOfTravel_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchDirectionOfTravel(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchIssue_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchIssue(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchIssue_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchIssue(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCurrentStatus_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchCurrentStatus(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCurrentStatus_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCurrentStatus(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCaseAim_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchCaseAim(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchCaseAim_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchCaseAim(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchDeEscalationPoint_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			await caseModelService.PatchDeEscalationPoint(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchDeEscalationPoint_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchDeEscalationPoint(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}

		[Test]
		public async Task WhenPatchNextSteps_ReturnsTask()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			await caseModelService.PatchNextSteps(CaseFactory.BuildPatchCaseModel());

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}

		[Test]
		public void WhenPatchNextSteps_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			// act
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchNextSteps(CaseFactory.BuildPatchCaseModel()));

			// assert
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
		
		[Test]
		public async Task WhenPatchCaseHistory_SetsCaseHistoryAndUpdatedAt()
		{
			// arrange
			var dateTimeStart = DateTimeOffset.Now;
			
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseService.Setup(cs => cs.GetCaseByUrn(caseDto.Urn)).ReturnsAsync(caseDto);

			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			// act
			await caseModelService.PatchCaseHistory((int)caseDto.Urn, caseDto.CreatedBy, "some test case history");

			// assert
			var timeStopped = DateTimeOffset.Now;
			mockCaseService
				.Verify(r => r.PatchCaseByUrn(It.Is<CaseDto>(dto => dto.CaseHistory.ToString() == "some test case history" && dto.UpdatedAt >= dateTimeStart && dto.UpdatedAt <= timeStopped)), 
					Times.Once);
		}

		[Test]
		public void WhenPatchCaseHistory_WhenPatchCaseThrowsException_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(new CaseDto());
			
			mockCaseService.Setup(cs => cs.PatchCaseByUrn(It.IsAny<CaseDto>()))
				.ThrowsAsync(new Exception("some error message"));
			
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			// act/ assert
			var exception = Assert.ThrowsAsync<Exception>(() => caseModelService.PatchCaseHistory(1, "some user", "some case history text"));
			Assert.That(exception!.Message, Is.EqualTo("some error message"));
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public async Task WhenPatchTerritory_SetsTerritoryAndUpdatedAt()
		{
			// arrange
			var dateTimeStart = DateTimeOffset.Now;
			
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();

			var caseDto = CaseFactory.BuildCaseDto();

			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseDto);

			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);
			
			// act
			await caseModelService.PatchTerritory((int)caseDto.Urn, caseDto.CreatedBy, Territory.South_And_South_East__London);

			// assert
			var timeStopped = DateTimeOffset.Now;
			mockCaseService
				.Verify(r => r.PatchCaseByUrn(It.Is<CaseDto>(dto => dto.Territory.ToString() == Territory.South_And_South_East__London.ToString() && dto.UpdatedAt >= dateTimeStart && dto.UpdatedAt <= timeStopped)), 
					Times.Once);
		}

		[Test]
		public void WhenPatchTerritory_WhenPatchCaseThrowsException_ThrowsException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			mockCaseService.Setup(cs => cs.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(new CaseDto());
			
			mockCaseService.Setup(cs => cs.PatchCaseByUrn(It.IsAny<CaseDto>()))
				.ThrowsAsync(new Exception("some error message"));
			
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			// act/ assert
			var exception = Assert.ThrowsAsync<Exception>(() => caseModelService.PatchTerritory(1, "some user", Territory.North_And_Utc__Utc));
			Assert.That(exception!.Message, Is.EqualTo("some error message"));
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Once);
		}
		
		[Test]
		public void WhenPatchTerritory_WhenCaseDtoNotFound_ThrowsNullReferenceException()
		{
			// arrange
			var mockRecordService = new Mock<IRecordService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var mockCaseService = new Mock<ICaseService>();
			
			var caseModelService = new CaseModelService(
				 mockRecordService.Object,
				 mockStatusCachedService.Object,
				 mockCaseService.Object,
				 mockLogger.Object);

			// act/ assert
			var exception = Assert.ThrowsAsync<NullReferenceException>(() => caseModelService.PatchTerritory(1, "some user", Territory.North_And_Utc__Utc));
			Assert.That(exception!.Message, Is.EqualTo("Object reference not set to an instance of an object."));
			mockCaseService.Verify(r => r.PatchCaseByUrn(It.IsAny<CaseDto>()), Times.Never);
		}
	}
}