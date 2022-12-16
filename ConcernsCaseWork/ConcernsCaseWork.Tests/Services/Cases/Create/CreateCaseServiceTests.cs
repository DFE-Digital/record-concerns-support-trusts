using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases.Create
{
	[Parallelizable(ParallelScope.All)]
	public class CreateCaseServiceTests
	{
		private readonly IFixture _fixture = new Fixture();
		
		[Test]
		public async Task WhenCreateCase_WithTrustUkPrn_CallsThePostCaseEndpointAndWipesCachedData()
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			mockRatingCachedService.Setup(r => r.GetDefaultRating()).ReturnsAsync(ratingDto);
				
			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;
			
			var initialUserState = new UserState(userName)
			{
				TrustUkPrn = trustUkPrn,
				CreateCaseModel = null
			};
	
			mockUserStateCachedService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(() => initialUserState);
			
			mockUserStateCachedService
				.Setup(s => s.StoreData(userName, It.IsAny<UserState>()))
				.Verifiable();

			mockCaseService
				.Setup(s => s.PostCase(It.IsAny<CreateCaseDto>()))
				.ReturnsAsync(
					new CaseDto
					{
						CreatedAt = createdAndUpdatedDate,
						UpdatedAt = createdAndUpdatedDate,
						ReviewAt = DateTimeOffset.MinValue,
						ClosedAt = DateTimeOffset.MinValue,
						CreatedBy = userName,
						Description = null,
						CrmEnquiry = null,
						TrustUkPrn = trustUkPrn, 
						ReasonAtReview = null,
						DeEscalation = DateTimeOffset.MinValue,
						Issue = null,
						CurrentStatus = null,
						CaseAim = null,
						DeEscalationPoint = null,
						NextSteps = null,
						CaseHistory = null,
						DirectionOfTravel = null,
						Urn = expectedNewCaseUrn,
						StatusId = statusDto.Id,
						RatingId = ratingDto.Id 
					});
			
			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockUserStateCachedService.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingCachedService.Object,
				mockSrmaService.Object
				);

			// act
			var createdCaseUrn = await createCaseService.CreateNonConcernsCase(userName);

			// assert
			Assert.That(createdCaseUrn, Is.EqualTo(expectedNewCaseUrn));
			
			mockCaseService
				.Verify(s => s.PostCase(It.Is<CreateCaseDto>(c => 
						c.Issue == null
                      && c.CaseAim == null
                      && c.CreatedAt >= createdAndUpdatedDate
                      && c.CreatedBy == userName
                      && c.CrmEnquiry == null
                      && c.CurrentStatus == null
                      && c.DeEscalation == DateTimeOffset.MinValue
                      && c.NextSteps == null
                      && c.RatingId == ratingDto.Id
                      && c.ReviewAt == DateTimeOffset.MinValue
                      && c.StatusId == statusDto.Id
                      && c.UpdatedAt >= createdAndUpdatedDate
                      && c.DeEscalationPoint == null
                      && c.DirectionOfTravel == null
                      && c.ReasonAtReview == null
                      && c.TrustUkPrn == trustUkPrn)), 
					Times.Once);

			mockLogger.VerifyLogInformationWasCalled("CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		[TestCase(null)]
		public void WhenCreateCase_WithNullTrustUkPrn_ErrorsAndDoesNotTryToCreateCase(string trustUkPrn)
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			mockRatingCachedService.Setup(r => r.GetDefaultRating()).ReturnsAsync(ratingDto);
				
			var userName = "some.user";
			
			mockUserStateCachedService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(() => new UserState(userName)
				{
					TrustUkPrn = trustUkPrn,
					CreateCaseModel = null
				});
			
			mockUserStateCachedService
				.Setup(s => s.StoreData(userName, It.IsAny<UserState>()))
				.Verifiable();

			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockUserStateCachedService.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingCachedService.Object,
				mockSrmaService.Object);

			// act
			var result = Assert.ThrowsAsync<Exception>(async () => await createCaseService.CreateNonConcernsCase(userName));

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.EqualTo("Cached TrustUkPrn is not set"));
			
			mockUserStateCachedService.Verify(s => s.GetData(userName), Times.Once);
			mockUserStateCachedService.Verify(s => s.StoreData(userName, It.IsAny<UserState>()), Times.Never);

			mockLogger.VerifyLogInformationWasCalled("CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasCalled("Cached TrustUkPrn is not set");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public void WhenCreateCase_ThrowsException_LogsAndRethrowsException()
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			mockRatingCachedService.Setup(r => r.GetDefaultRating()).ReturnsAsync(ratingDto);
				
			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;
			
			var initialUserState = new UserState(userName)
			{
				TrustUkPrn = trustUkPrn,
				CreateCaseModel = null
			};
	
			mockUserStateCachedService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(() => initialUserState);
			
			mockUserStateCachedService
				.Setup(s => s.StoreData(userName, It.IsAny<UserState>()))
				.Verifiable();

			mockCaseService
				.Setup(s => s.PostCase(It.IsAny<CreateCaseDto>()))
				.ReturnsAsync(
					new CaseDto
					{
						CreatedAt = createdAndUpdatedDate,
						UpdatedAt = createdAndUpdatedDate,
						ReviewAt = DateTimeOffset.MinValue,
						ClosedAt = DateTimeOffset.MinValue,
						CreatedBy = userName,
						Description = null,
						CrmEnquiry = null,
						TrustUkPrn = trustUkPrn, 
						ReasonAtReview = null,
						DeEscalation = DateTimeOffset.MinValue,
						Issue = null,
						CurrentStatus = null,
						CaseAim = null,
						DeEscalationPoint = null,
						NextSteps = null,
						CaseHistory = null,
						DirectionOfTravel = null,
						Urn = expectedNewCaseUrn,
						StatusId = statusDto.Id,
						RatingId = ratingDto.Id 
					});

			mockSrmaService.Setup(s => s.SaveSRMA(It.IsAny<SRMAModel>())).Throws(new Exception("some error happened"));
			
			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockUserStateCachedService.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingCachedService.Object,
				mockSrmaService.Object
				);

			var srmaModel = new SRMAModel();

			// act
			var result = Assert.ThrowsAsync<Exception>(() => createCaseService.CreateNonConcernsCase(userName, srmaModel));

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.EqualTo("some error happened"));

			mockLogger.VerifyLogInformationWasCalled(Times.Exactly(2) ,"CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasCalled("Exception - some error happened");
			mockLogger.VerifyNoOtherCalls();
			
			mockSrmaService.Verify(s => s.SaveSRMA(srmaModel), Times.Once);
		}
				
		[Test]
		public async Task WhenCreateCase_WithSRMA_CreatesACaseAndSrma()
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			mockRatingCachedService.Setup(r => r.GetDefaultRating()).ReturnsAsync(ratingDto);
				
			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;
			
			var initialUserState = new UserState(userName)
			{
				TrustUkPrn = trustUkPrn,
				CreateCaseModel = null
			};
	
			mockUserStateCachedService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(() => initialUserState);
			
			mockUserStateCachedService
				.Setup(s => s.StoreData(userName, It.IsAny<UserState>()))
				.Verifiable();

			mockCaseService
				.Setup(s => s.PostCase(It.IsAny<CreateCaseDto>()))
				.ReturnsAsync(
					new CaseDto
					{
						CreatedAt = createdAndUpdatedDate,
						UpdatedAt = createdAndUpdatedDate,
						ReviewAt = DateTimeOffset.MinValue,
						ClosedAt = DateTimeOffset.MinValue,
						CreatedBy = userName,
						Description = null,
						CrmEnquiry = null,
						TrustUkPrn = trustUkPrn, 
						ReasonAtReview = null,
						DeEscalation = DateTimeOffset.MinValue,
						Issue = null,
						CurrentStatus = null,
						CaseAim = null,
						DeEscalationPoint = null,
						NextSteps = null,
						CaseHistory = null,
						DirectionOfTravel = null,
						Urn = expectedNewCaseUrn,
						StatusId = statusDto.Id,
						RatingId = ratingDto.Id 
					});

			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockUserStateCachedService.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingCachedService.Object,
				mockSrmaService.Object
				);

			var srmaModel = new SRMAModel();

			// act
			var createdCaseUrn = await createCaseService.CreateNonConcernsCase(userName, srmaModel);

			// assert
			Assert.That(createdCaseUrn, Is.EqualTo(expectedNewCaseUrn));

			mockLogger.VerifyLogInformationWasCalled(Times.Exactly(2) ,"CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
			
			mockSrmaService.Verify(s => s.SaveSRMA(srmaModel), Times.Once);
			
			mockCaseService
				.Verify(s => s.PostCase(It.Is<CreateCaseDto>(c => 
						c.Issue == null
						&& c.CaseAim == null
						&& c.CreatedAt >= createdAndUpdatedDate
						&& c.CreatedBy == userName
						&& c.CrmEnquiry == null
						&& c.CurrentStatus == null
						&& c.DeEscalation == DateTimeOffset.MinValue
						&& c.NextSteps == null
						&& c.RatingId == ratingDto.Id
						&& c.ReviewAt == DateTimeOffset.MinValue
						&& c.StatusId == statusDto.Id
						&& c.UpdatedAt >= createdAndUpdatedDate
						&& c.DeEscalationPoint == null
						&& c.DirectionOfTravel == null
						&& c.ReasonAtReview == null
						&& c.TrustUkPrn == trustUkPrn)), 
					Times.Once);
		}
	}
}