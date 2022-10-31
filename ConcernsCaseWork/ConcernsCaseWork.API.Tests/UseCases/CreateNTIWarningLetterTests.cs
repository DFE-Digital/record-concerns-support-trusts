using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter;
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
    public class CreateNTIWarningLetterTests
	{
	    [Fact]
        public void CreateNTIWarningLetter_ShouldCreateAndReturnFinancialPlanResponse_WhenGivenCreateNTIWarningLetterRequest()
        {
			var caseUrn = 544;
			var now = DateTime.Now;
			var statusId = 1;
			var notes = "Notes";
			var createdBy = "Test User";
			var reasons = new List<int>() { 1, 3 };
			var conditions = new List<int>() { 1, 3 };

			var createWarningLetterequest = Builder<CreateNTIWarningLetterRequest>
	            .CreateNew()
	            .With(r => r.CaseUrn = caseUrn)
	            .With(r => r.StatusId = statusId)
				.With(r => r.CreatedAt = now)
	            .With(r => r.DateLetterSent = now)
				.With(r => r.Notes = notes)
	            .With(r => r.CreatedBy = createdBy)
	            .With(r => r.WarningLetterReasonsMapping = reasons)
	            .With(r => r.WarningLetterConditionsMapping = conditions)
				.Build();

			var warningLetterDbModel = new NTIWarningLetter
			{
				CaseUrn = caseUrn,
				StatusId = statusId,
				DateLetterSent = now,
				CreatedAt = now,
				Notes = notes,
				CreatedBy = createdBy,
				WarningLetterReasonsMapping = reasons.Select(r => {
					return new NTIWarningLetterReasonMapping()
					{
						NTIWarningLetterReasonId = r
					};
				}).ToList(),
				WarningLetterConditionsMapping = conditions.Select(r => {
					return new NTIWarningLetterConditionMapping()
					{
						NTIWarningLetterConditionId = r
					};
				}).ToList()
			};

            var expectedResult = new NTIWarningLetterResponse
            {
				CaseUrn = caseUrn,
				StatusId = statusId,
				DateLetterSent = now,
				CreatedAt = now,
				Notes = notes,
				CreatedBy = createdBy,
				WarningLetterReasonsMapping = reasons,
				WarningLetterConditionsMapping = conditions
			};
			
			var mockGateway = new Mock<INTIWarningLetterGateway>();
            
            mockGateway.Setup(g => g.CreateNTIWarningLetter(It.IsAny<NTIWarningLetter>())).Returns(Task.FromResult(warningLetterDbModel));
            
            var useCase = new CreateNTIWarningLetter(mockGateway.Object);
            
            var result = useCase.Execute(createWarningLetterequest);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}