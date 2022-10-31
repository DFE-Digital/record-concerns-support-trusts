using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
    public class ConcernsCaseControllerTests
    {
        private Mock<ILogger<ConcernsCaseController>> mockLogger = new Mock<ILogger<ConcernsCaseController>>();
        
        [Fact]
        public void CreateConcernsCase_Returns201WhenSuccessfullyCreatesAConcernsCase()
        {
            var createConcernsCase = new Mock<ICreateConcernsCase>();
            var createConcernsCaseRequest = Builder<ConcernCaseRequest>
                .CreateNew()
                .With(cr => cr.RatingUrn = 123)
                .Build();

            var concernsCaseResponse = Builder<ConcernsCaseResponse>
                .CreateNew().Build();

            createConcernsCase.Setup(a => a.Execute(createConcernsCaseRequest))
                .Returns(concernsCaseResponse);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                createConcernsCase.Object, 
                null, 
                null, 
                null,
                null
            );
            
            var result = controller.Create(createConcernsCaseRequest);
            
            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);
            
            result.Result.Should().BeEquivalentTo(new ObjectResult(expected) {StatusCode = StatusCodes.Status201Created});
        }
        
        [Fact]
        public void GetConcernsCaseByUrn_ReturnsNotFound_WhenConcernsCaseIsNotFound()
        {
            var urn = 100;
            
            var getConcernsCaseByUrn = new Mock<IGetConcernsCaseByUrn>();
            

            getConcernsCaseByUrn.Setup(a => a.Execute(urn))
                .Returns(() => null);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                getConcernsCaseByUrn.Object, 
                null,
                null,
                null
            );
            
            var result = controller.GetByUrn(urn);
            result.Result.Should().BeEquivalentTo(new NotFoundResult());
        }
        
        [Fact]
        public void GetConcernsCaseByUrn_Returns200AndTheFoundConcernsCase_WhenSuccessfullyGetsAConcernsCaseByUrn()
        {
            var getConcernsCaseByUrn = new Mock<IGetConcernsCaseByUrn>();
            var urn = 123;

            var concernsCaseResponse = Builder<ConcernsCaseResponse>
                .CreateNew().Build();

            getConcernsCaseByUrn.Setup(a => a.Execute(urn))
                .Returns(concernsCaseResponse);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                getConcernsCaseByUrn.Object, 
                null,
                null, 
                null
            );
            
            var result = controller.GetByUrn(urn);
            
            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);
            
            result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
        }

        [Fact]
        public void GetConcernsCaseByTrustUkprn_Returns200AndTheFoundConcernsCase_WhenSuccessfullyGetsAConcernsCaseByTrustUkprn()
        {
            var getConcernsCaseByTrustUkprn = new Mock<IGetConcernsCaseByTrustUkprn>();
            var trustUkprn = "100008";

            var concernsCaseResponse = Builder<ConcernsCaseResponse>
                .CreateNew().Build();

            getConcernsCaseByTrustUkprn.Setup(a => a.Execute(trustUkprn, 1, 10))
                .Returns(new List<ConcernsCaseResponse>{concernsCaseResponse});

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                null, 
                getConcernsCaseByTrustUkprn.Object,
                null,
                null
            );
            
            var result = controller.GetByTrustUkprn(trustUkprn, 1, 10);
            var expectedPagingResponse = new PagingResponse
            {
                Page = 1,
                RecordCount = 1,
                NextPageUrl = null
            };
            var expected = new ApiResponseV2<ConcernsCaseResponse>(new List<ConcernsCaseResponse>{concernsCaseResponse}, expectedPagingResponse);
            
            result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
        }
        
        [Fact]
        public void UpdateConcernsCase_Returns200AndTheUpdatedConcernsCase_WhenSuccessfullyGetsAConcernsCase()
        {
            var updateConcernsCase = new Mock<IUpdateConcernsCase>();
            var urn = 456;
            var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
                .With(cr => cr.RatingUrn = 123)
                .Build();
            
            var concernsCaseResponse = Builder<ConcernsCaseResponse>
                .CreateNew().Build();

            updateConcernsCase.Setup(a => a.Execute(urn, updateRequest))
                .Returns(concernsCaseResponse);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                null, 
                null,
                updateConcernsCase.Object,
                null
            );
            
            var result = controller.Update(urn, updateRequest);
           
            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCaseResponse);
            
            result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
        }
        
        [Fact]
        public void UpdateConcernsCase_Returns404NotFound_WhenNoConcernsCaseIsFound()
        {
            var updateConcernsCase = new Mock<IUpdateConcernsCase>();
            var urn = 7;
            var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
                .With(cr => cr.RatingUrn = 123)
                .Build();
            

            updateConcernsCase.Setup(a => a.Execute(urn, updateRequest))
                .Returns(() => null);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                null, 
                null,
                updateConcernsCase.Object,
                null
            );
            
            var result = controller.Update(urn, updateRequest);

            result.Result.Should().BeEquivalentTo(new NotFoundResult());
        }
        
        
         
        [Fact]
        public void GetConcernsCaseByOwnerId_ReturnsCaseResponses_WhenConcernsCasesAreFound()
        {
            var getConcernsCaseByOwnerId = new Mock<IGetConcernsCasesByOwnerId>();
            var ownerId = "567";
            var statusUrn = 567;
            var response = Builder<ConcernsCaseResponse>.CreateListOfSize(4).Build();

            getConcernsCaseByOwnerId.Setup(a => a.Execute(ownerId, statusUrn, 1, 50))
                .Returns(response);

            var controller = new ConcernsCaseController(
                mockLogger.Object,
                null, 
                null, 
                null,
                null,
                getConcernsCaseByOwnerId.Object
            );
            var result = controller.GetByOwnerId(ownerId, statusUrn, 1, 50);
            
            var expectedPagingResponse = new PagingResponse
            {
                Page = 1,
                RecordCount = 4,
                NextPageUrl = null
            };
            var expected = new ApiResponseV2<ConcernsCaseResponse>(response, expectedPagingResponse);
            result.Result.Should().BeEquivalentTo(new OkObjectResult(expected));
        }
    }
}