using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Ratings;
using Service.Redis.Status;
using Service.Redis.Users;
using Service.TRAMS.Cases;
using Service.TRAMS.Ratings;
using Service.TRAMS.Status;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases.Create
{
	[Parallelizable(ParallelScope.All)]
	public class CreateCaseServiceTests
	{
		[Test]
		public async Task WhenPostCase_WithTrustUkPrn_CallsThePostCaseEndpointAndWipesCachedData()
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseCachedService = new Mock<ICaseCachedService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			mockRatingCachedService.Setup(r => r.GetDefaultRating()).ReturnsAsync(ratingDto);
				
			var userName = "some.user";
			var trustUkPrn = "someprn";
			var expectedNewCaseUrn = 45;
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

			mockCaseCachedService
				.Setup(s => s.PostCase(It.IsAny<CreateCaseDto>()))
				.ReturnsAsync(
					new CaseDto(
						createdAndUpdatedDate, 
						createdAndUpdatedDate, 
						DateTimeOffset.MinValue, 
						DateTimeOffset.MinValue, 
						userName, null, null, 
						trustUkPrn, null, 
						DateTimeOffset.MinValue, null, null, null, null, null, null, 
						expectedNewCaseUrn, 
						statusDto.Urn, 
						ratingDto.Urn));
			
			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockUserStateCachedService.Object,
				mockStatusCachedService.Object,
				mockCaseCachedService.Object,
				mockRatingCachedService.Object
				);

			// act
			var createdCaseUrn = await createCaseService.CreateNonConcernsCase(userName);

			// assert
			Assert.That(createdCaseUrn, Is.EqualTo(expectedNewCaseUrn));
			
			mockCaseCachedService
				.Verify(s => s.PostCase(It.Is<CreateCaseDto>(c => 
						c.Issue == null
                      && c.CaseAim == null
                      && c.ClosedAt == DateTimeOffset.MinValue
                      && c.CreatedAt >= createdAndUpdatedDate
                      && c.CreatedBy == userName
                      && c.CrmEnquiry == null
                      && c.CurrentStatus == null
                      && c.DeEscalation == DateTimeOffset.MinValue
                      && c.NextSteps == null
                      && c.RatingUrn == ratingDto.Urn
                      && c.ReviewAt == DateTimeOffset.MinValue
                      && c.StatusUrn == statusDto.Urn
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
		public void WhenPostCase_WithNullTrustUkPrn_ErrorsAndDoesNotTryToCreateCase(string trustUkPrn)
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingCachedService = new Mock<IRatingCachedService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseCachedService = new Mock<ICaseCachedService>();

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
				mockCaseCachedService.Object,
				mockRatingCachedService.Object);

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
	}
}