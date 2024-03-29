using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Features.Case;
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
		private readonly Mock<ILogger<ConcernsCaseController>> _mockLogger = new Mock<ILogger<ConcernsCaseController>>();

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
	}
}