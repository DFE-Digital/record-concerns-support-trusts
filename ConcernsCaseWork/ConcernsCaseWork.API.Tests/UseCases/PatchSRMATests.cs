using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases.CaseActions.SRMA;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using SRMAStatus = ConcernsCaseWork.Data.Enums.SRMAStatus;

namespace ConcernsCaseWork.API.Tests.UseCases
{
    public class PatchSRMATests
	{
	    [Fact]
        public void PatchSRMA_ShouldPatchSRMAStatusAndReturnSRMAResponse_WhenGivenPatchSRMARequest()
        {
            var targetStatus = SRMAStatus.Deployed;
            var srmaDbModel = CreateSRMACase();

            Func<SRMACase, SRMACase> patchStatusDelegate = input =>
            {
                input.StatusId = (int)targetStatus;
                return input;
            };

            var mockGateway = new Mock<ISRMAGateway>();

            mockGateway.Setup(g => g.PatchSRMAAsync(srmaDbModel.Id, patchStatusDelegate)).Returns(Task.FromResult(patchStatusDelegate(srmaDbModel)));
            
            var useCase = new PatchSRMA(mockGateway.Object);
            
            var result = useCase.Execute(new PatchSRMARequest { SRMAId = srmaDbModel.Id, Delegate = patchStatusDelegate});

            result.Should().NotBeNull();
            result.Status.Should().BeEquivalentTo(targetStatus);
        }

        [Fact]
        public void PatchSRMA_ShouldPatchSRMANotesAndReturnSRMAResponse_WhenGivenPatchSRMARequest()
        {
            var targetNotes = "Notes - hello world";
            var srmaDbModel = CreateSRMACase();

            Func<SRMACase, SRMACase> patchStatusDelegate = input =>
            {
                input.Notes = targetNotes;
                return input;
            };

            var mockGateway = new Mock<ISRMAGateway>();

            mockGateway.Setup(g => g.PatchSRMAAsync(srmaDbModel.Id, patchStatusDelegate)).Returns(Task.FromResult(patchStatusDelegate(srmaDbModel)));

            var useCase = new PatchSRMA(mockGateway.Object);

            var result = useCase.Execute(new PatchSRMARequest { SRMAId = srmaDbModel.Id, Delegate = patchStatusDelegate });

            result.Should().NotBeNull();
            result.Notes.Should().BeEquivalentTo(targetNotes);
        }

        private SRMACase CreateSRMACase()
        {
            var status = SRMAStatus.PreparingForDeployment;
            var reason = SRMAReasonOffered.OfferLinked;
            var datetOffered = DateTime.Now.AddDays(-5);

            return new SRMACase
            {
                Id = 834,
                StatusId = (int)status,
                DateOffered = datetOffered,
                ReasonId = (int)reason
            };
        }
    }
}