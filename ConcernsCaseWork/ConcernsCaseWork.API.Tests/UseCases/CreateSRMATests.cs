using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases.CaseActions.SRMA;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using SRMAStatus = ConcernsCaseWork.Data.Enums.SRMAStatus;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class CreateSRMATests
	{
	    [Fact]
        public void CreateSRMA_ShouldCreateAndReturnSRMAResponse_WhenGivenCreateSRMARequest()
        {
			var status = SRMAStatus.Deployed;
			var datetOffered = DateTime.Now.AddDays(-5);

			var createSRMARequest = Builder<CreateSRMARequest>
	            .CreateNew()
	            .With(r => r.Status = status)
	            .With(r => r.DateOffered = datetOffered)
	            .Build();

			var srmaDbModel = new SRMACase
			{
				StatusId = (int)status,
				DateOffered = datetOffered
            };

            var expectedResult = new SRMAResponse
            {
				DateOffered = datetOffered,
				Status = status
			};
			
			var mockGateway = new Mock<ISRMAGateway>();
            
            mockGateway.Setup(g => g.CreateSRMA(It.IsAny<SRMACase>())).Returns(Task.FromResult(srmaDbModel));
            
            var useCase = new CreateSRMA(mockGateway.Object);
            
            var result = useCase.Execute(createSRMARequest);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}