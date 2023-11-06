using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using FluentAssertions;
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
			var mockRatingService = new Mock<IRatingService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);

			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var trustCompaniesHouseNumber = _fixture.CreateMany<char>(8).ToString();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;

			var initialUserState = new UserState(userName) { TrustUkPrn = trustUkPrn, CreateCaseModel = null };

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
						RatingId = ratingDto.Id,
					});

			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingService.Object,
				mockSrmaService.Object
			);

			// act
			var createdCaseUrn = await createCaseService.CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber);

			// assert
			Assert.That(createdCaseUrn, Is.EqualTo(expectedNewCaseUrn));

			mockLogger.VerifyLogInformationWasCalled("CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[TestCase(null, "12345678", "12345678")]
		[TestCase("john.smith", null, "12345678")]
		[TestCase("john.smith", "12345678", null)]
		public async Task WhenCreateCase_WithNullTrustUkPrn_ErrorsAndDoesNotTryToCreateCase(string userName, string companiesHouseNumber, string trustUkPrn)
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingService = new Mock<IRatingService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockTrustService = new Mock<ITrustService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);
			
			var mockTrust = new Mock<TrustDetailsDto>();
			mockTrust.Setup(x => x.GiasData.UkPrn).Returns(trustUkPrn);
			mockTrust.Setup(x => x.GiasData.CompaniesHouseNumber).Returns(companiesHouseNumber);
			mockTrustService.Setup(x => x.GetTrustByUkPrn(trustUkPrn)).ReturnsAsync(mockTrust.Object);

			var createCaseService = new CreateCaseService(
				mockLogger.Object,
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingService.Object,
				mockSrmaService.Object);

			// act
			Func<Task> act = async () => await createCaseService.CreateNonConcernsCase(userName, trustUkPrn, companiesHouseNumber);
			
			// Assert
			if (userName is null)
			{
				await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'userName')");
			}
			else if (trustUkPrn is null)
			{
				await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'trustUkPrn')");
			}
			else if (companiesHouseNumber is null)
			{
				await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'trustCompaniesHouseNumber')");
			}

			mockUserStateCachedService.Verify(s => s.GetData(userName), Times.Never);
			mockUserStateCachedService.Verify(s => s.StoreData(userName, It.IsAny<UserState>()), Times.Never);
		}

		[Test]
		public void WhenCreateCase_ThrowsException_LogsAndRethrowsException()
		{
			// arrange
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<CreateCaseService>>();
			var mockRatingService = new Mock<IRatingService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);

			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var trustCompaniesHouseNumber = _fixture.CreateMany<char>(8).ToString();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;

			var initialUserState = new UserState(userName) { TrustUkPrn = trustUkPrn, CreateCaseModel = null };

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
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingService.Object,
				mockSrmaService.Object
			);

			var srmaModel = new SRMAModel();

			// act
			var result = Assert.ThrowsAsync<Exception>(() => createCaseService.CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber, srmaModel));

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.EqualTo("some error happened"));

			mockLogger.VerifyLogInformationWasCalled(Times.Exactly(2), "CreateNonConcernsCase");
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
			var mockRatingService = new Mock<IRatingService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockCaseService = new Mock<ICaseService>();
			var mockSrmaService = new Mock<ISRMAService>();

			var ratingDto = new RatingDto("N/A", DateTimeOffset.Now, DateTimeOffset.Now, 1);
			var statusDto = new StatusDto("some status", DateTimeOffset.Now, DateTimeOffset.Now, 2);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString())).ReturnsAsync(statusDto);

			var userName = _fixture.Create<string>();
			var trustUkPrn = _fixture.Create<string>();
			var trustCompaniesHouseNumber = _fixture.CreateMany<char>(8).ToString();
			var expectedNewCaseUrn = _fixture.Create<long>();
			var createdAndUpdatedDate = DateTimeOffset.Now;

			var initialUserState = new UserState(userName) { TrustUkPrn = trustUkPrn, CreateCaseModel = null };

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
				mockStatusCachedService.Object,
				mockCaseService.Object,
				mockRatingService.Object,
				mockSrmaService.Object
			);

			var srmaModel = new SRMAModel();

			// act
			var createdCaseUrn = await createCaseService.CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber, srmaModel);

			// assert
			Assert.That(createdCaseUrn, Is.EqualTo(expectedNewCaseUrn));

			mockLogger.VerifyLogInformationWasCalled(Times.Exactly(2), "CreateNonConcernsCase");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();

			mockSrmaService.Verify(s => s.SaveSRMA(srmaModel), Times.Once);
		}
	}
}