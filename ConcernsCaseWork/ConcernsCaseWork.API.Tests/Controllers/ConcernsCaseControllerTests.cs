using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
	public class ConcernsCaseControllerTests
	{
		private readonly Mock<ILogger<ConcernsCaseController>> _mockLogger = new();
		private readonly Mock<ICreateConcernsCase> _createConcernsCase = new();
		private readonly Mock<IGetConcernsCaseByUrn> _getConcernsCaseByUrn = new();
		private readonly Mock<IGetConcernsCaseByTrustUkprn> _getConcernsCaseByTrustUkprn = new();
		private readonly Mock<IUpdateConcernsCase> _updateConcernsCase = new();
		private readonly Mock<IDeleteConcernsCase> _deleteConcernsCase = new();
		private readonly Mock<IGetConcernsCasesByOwnerId> _getConcernsCasesByOwnerId = new();
		private readonly Mock<IGetActiveConcernsCaseSummariesForUsersTeam> _getActiveConcernsCaseSummariesForUsersTeam = new();
		private readonly Mock<IGetConcernsCaseSummariesByFilter> _getConcernsCaseSummariesByFilter = new();
		private readonly Mock<IGetActiveConcernsCaseSummariesByOwner> _getActiveConcernsCaseSummariesByOwner = new();
		private readonly Mock<IGetClosedConcernsCaseSummariesByOwner> _getClosedConcernsCaseSummariesByOwner = new();
		private readonly Mock<IGetActiveConcernsCaseSummariesByTrust> _getActiveConcernsCaseSummariesByTrust = new();
		private readonly Mock<IGetClosedConcernsCaseSummariesByTrust> _getClosedConcernsCaseSummariesByTrust = new();

		private ConcernsCaseController CreateController()
		{
			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				_createConcernsCase.Object,
				_getConcernsCaseByUrn.Object,
				_getConcernsCaseByTrustUkprn.Object,
				_updateConcernsCase.Object,
				_deleteConcernsCase.Object,
				_getConcernsCasesByOwnerId.Object,
				_getClosedConcernsCaseSummariesByOwner.Object,
				_getActiveConcernsCaseSummariesByTrust.Object,
				_getClosedConcernsCaseSummariesByTrust.Object,
				_getActiveConcernsCaseSummariesForUsersTeam.Object,
				_getActiveConcernsCaseSummariesByOwner.Object,
				_getConcernsCaseSummariesByFilter.Object
			);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			};
			return controller;
		}

		[Fact]
		public async Task CreateConcernsCase_Returns201WhenSuccessfullyCreatesAConcernsCase()
		{
			var createConcernsCase = new Mock<ICreateConcernsCase>();
			var createConcernsCaseRequest = Builder<ConcernCaseRequest>
				.CreateNew()
				.With(cr => cr.RatingId = 123)
				.Build();

			var concernsCaseResponse = Builder<ConcernsCaseResponse>
				.CreateNew().Build();

			createConcernsCase.Setup(a => a.Execute(createConcernsCaseRequest))
				.Returns(concernsCaseResponse);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				createConcernsCase.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.Create(createConcernsCaseRequest);

			var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);

			result.Result.Should().BeEquivalentTo(new ObjectResult(expected) { StatusCode = StatusCodes.Status201Created });
		}

		[Fact]
		public async Task GetConcernsCaseByUrn_ReturnsNotFound_WhenConcernsCaseIsNotFound()
		{
			var urn = 100;

			var getConcernsCaseByUrn = new Mock<IGetConcernsCaseByUrn>();


			getConcernsCaseByUrn.Setup(a => a.Execute(urn))
				.Returns(() => null);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				getConcernsCaseByUrn.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.GetByUrn(urn);
			result.Result.Should().BeEquivalentTo(new NotFoundResult());
		}

		[Fact]
		public async Task GetConcernsCaseByUrn_Returns200AndTheFoundConcernsCase_WhenSuccessfullyGetsAConcernsCaseByUrn()
		{
			var getConcernsCaseByUrn = new Mock<IGetConcernsCaseByUrn>();
			var urn = 123;

			var concernsCaseResponse = Builder<ConcernsCaseResponse>
				.CreateNew().Build();

			getConcernsCaseByUrn.Setup(a => a.Execute(urn))
				.Returns(concernsCaseResponse);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				getConcernsCaseByUrn.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.GetByUrn(urn);

			var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);

			result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
		}

		[Fact]
		public async Task GetConcernsCaseByTrustUkprn_Returns200AndTheFoundConcernsCase_WhenSuccessfullyGetsAConcernsCaseByTrustUkprn()
		{
			var getConcernsCaseByTrustUkprn = new Mock<IGetConcernsCaseByTrustUkprn>();
			var trustUkprn = "100008";

			var concernsCaseResponse = Builder<ConcernsCaseResponse>
				.CreateNew().Build();

			getConcernsCaseByTrustUkprn.Setup(a => a.Execute(trustUkprn, 1, 10))
				.Returns(new List<ConcernsCaseResponse> { concernsCaseResponse });

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				null,
				getConcernsCaseByTrustUkprn.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.GetByTrustUkprn(trustUkprn, 1, 10);
			var expectedPagingResponse = new PagingResponse
			{
				Page = 1,
				RecordCount = 1,
				NextPageUrl = null,
				TotalPages = 1
			};
			var expected = new ApiResponseV2<ConcernsCaseResponse>(new List<ConcernsCaseResponse> { concernsCaseResponse }, expectedPagingResponse);

			result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
		}

		[Fact]
		public async Task UpdateConcernsCase_Returns200AndTheUpdatedConcernsCase_WhenSuccessfullyGetsAConcernsCase()
		{
			var updateConcernsCase = new Mock<IUpdateConcernsCase>();
			var urn = 456;
			var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
				.With(cr => cr.RatingId = 123)
				.Build();

			var concernsCaseResponse = Builder<ConcernsCaseResponse>
				.CreateNew().Build();

			updateConcernsCase.Setup(a => a.Execute(urn, updateRequest))
				.Returns(concernsCaseResponse);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				null,
				null,
				updateConcernsCase.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.Update(urn, updateRequest);

			var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);

			result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
		}

		[Fact]
		public async Task UpdateConcernsCase_Returns404NotFound_WhenNoConcernsCaseIsFound()
		{
			var updateConcernsCase = new Mock<IUpdateConcernsCase>();
			var urn = 7;
			var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
				.With(cr => cr.RatingId = 123)
				.Build();


			updateConcernsCase.Setup(a => a.Execute(urn, updateRequest))
				.Returns(() => null);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				null,
				null,
				updateConcernsCase.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var result = await controller.Update(urn, updateRequest);

			result.Result.Should().BeEquivalentTo(new NotFoundResult());
		}

		[Fact]
		public async Task GetConcernsCaseByOwnerId_ReturnsCaseResponses_WhenConcernsCasesAreFound()
		{
			var getConcernsCaseByOwnerId = new Mock<IGetConcernsCasesByOwnerId>();
			var ownerId = "567";
			var statusId = 567;
			var response = Builder<ConcernsCaseResponse>.CreateListOfSize(4).Build();

			getConcernsCaseByOwnerId.Setup(a => a.Execute(ownerId, statusId, 1, 50))
				.Returns(response);

			var controller = new ConcernsCaseController(
				_mockLogger.Object,
				null,
				null,
				null,
				null,
				null,
				getConcernsCaseByOwnerId.Object,
				null,
				null,
				null,
				null,
				null,
				null
			);
			var result = await controller.GetByOwnerId(ownerId, statusId, 1, 50);

			var expectedPagingResponse = new PagingResponse
			{
				Page = 1,
				RecordCount = 4,
				NextPageUrl = null,
				TotalPages = 1
			};
			var expected = new ApiResponseV2<ConcernsCaseResponse>(response, expectedPagingResponse);
			result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
		}

		[Fact]
		public async Task GetAllSummariesByFilter_ReturnsOk_WithPaging()
		{
			// Arrange
			var regions = new[] { Region.London };
			var statuses = new[] { CaseStatus.Live };
			int? page = 1;
			int? count = 10;

			var summaries = new List<ActiveCaseSummaryResponse>
			{
				new() { TeamLedBy = "A", Rating = new ConcernsRatingResponse { Name = "High" } }
			};

			int recordCount = 1;

			_getConcernsCaseSummariesByFilter
				.Setup(x => x.Execute(It.IsAny<GetCaseSummariesByFilterParameters>()))
				.ReturnsAsync((summaries, recordCount));

			var controller = CreateController();

			// Act
			var result = await controller.GetAllSummariesByFilter(regions, statuses, page, count);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var apiResponse = Assert.IsType<ApiResponseV2<ActiveCaseSummaryResponse>>(okResult.Value);

			Assert.Single(apiResponse.Data);
			Assert.NotNull(apiResponse.Paging);
			Assert.Equal(recordCount, apiResponse.Paging.RecordCount);
		}
	}
}