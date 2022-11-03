using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class CreateNTIUnderConsiderationTests
	{
	    [Fact]
        public void CreateCreateNTIUnderConsideration_ShouldCreateAndReturnNTIUnderConsiderationResponse_WhenGivenCreateNTIUnderConsiderationRequest()
        {
			var caseUrn = 544;
			var now = DateTime.Now;
			var notes = "Notes";
			var createdBy = "Test User";
			var reasons = new List<int>() { 1, 3 };

			var createConsiderationRequest = Builder<CreateNTIUnderConsiderationRequest>
	            .CreateNew()
	            .With(r => r.CaseUrn = caseUrn)
	            .With(r => r.CreatedAt = now)
	            .With(r => r.Notes = notes)
	            .With(r => r.CreatedBy = createdBy)
	            .With(r => r.UnderConsiderationReasonsMapping = reasons)
				.Build();

			var considerationDbModel = new NTIUnderConsideration
			{
				CaseUrn = caseUrn,
				CreatedAt = now,
				Notes = notes,
				CreatedBy = createdBy,
				UnderConsiderationReasonsMapping = reasons.Select(r => {
					return new NTIUnderConsiderationReasonMapping()
					{
						NTIUnderConsiderationReasonId = r
					};
				}).ToList()
			};

            var expectedResult = new NTIUnderConsiderationResponse
            {
				CaseUrn = caseUrn,
				CreatedAt = now,
				Notes = notes,
				CreatedBy = createdBy,
				UnderConsiderationReasonsMapping = reasons
			};
			
			var mockGateway = new Mock<INTIUnderConsiderationGateway>();
            
            mockGateway.Setup(g => g.CreateNTIUnderConsideration(It.IsAny<NTIUnderConsideration>())).Returns(Task.FromResult(considerationDbModel));
            
            var useCase = new CreateNTIUnderConsideration(mockGateway.Object);
            
            var result = useCase.Execute(createConsiderationRequest);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}