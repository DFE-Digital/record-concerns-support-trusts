using Xunit;
using Moq;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Tests.Features.Case
{
    public class UpdateConcernsCaseTests
    {
		[Fact]
		public void Execute_ShouldUpdateCaseAndReturnResponse_WhenRatingIdUnchanged()
		{
			// Arrange
			var gatewayMock = new Mock<IConcernsCaseGateway>();
			var historyGatewayMock = new Mock<IConcernsCaseRiskToTrustRatingHistoryGateway>();

			var caseId = 1;
			var ratingId = 2;
			var concernsCase = new ConcernsCase { Id = caseId, RatingId = ratingId, RatingRationalCommentary = "Old" };
			var request = new ConcernCaseRequest { RatingId = ratingId, RatingRationalCommentary = "New comment but with old rating" };

			gatewayMock.Setup(g => g.GetConcernsCaseByUrn(It.IsAny<int>(), false)).Returns(concernsCase);
			gatewayMock.Setup(g => g.Update(It.IsAny<ConcernsCase>())).Returns(concernsCase);

			var sut = new UpdateConcernsCase(gatewayMock.Object, historyGatewayMock.Object);

			// Act
			var result = sut.Execute(caseId, request);

			// Assert
			historyGatewayMock.Verify(h => h.CreateHistory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			gatewayMock.Verify(g => g.Update(It.IsAny<ConcernsCase>()), Times.Once);
			Assert.NotNull(result);
		}

		[Fact]
		public void Execute_ShouldCreateHistory_WhenRatingIdChanged()
		{
			// Arrange
			var gatewayMock = new Mock<IConcernsCaseGateway>();
			var historyGatewayMock = new Mock<IConcernsCaseRiskToTrustRatingHistoryGateway>();

			var caseId = 1;
			var oldRatingId = 2;
			var newRatingId = 3;
			var concernsCase = new ConcernsCase { Id = caseId, RatingId = oldRatingId, RatingRationalCommentary = "Old" };
			var request = new ConcernCaseRequest { RatingId = newRatingId, RatingRationalCommentary = "Old comment with new rating" };

			gatewayMock.Setup(g => g.GetConcernsCaseByUrn(It.IsAny<int>(), false)).Returns(concernsCase);
			gatewayMock.Setup(g => g.Update(It.IsAny<ConcernsCase>())).Returns(concernsCase);

			var sut = new UpdateConcernsCase(gatewayMock.Object, historyGatewayMock.Object);

			// Act
			var result = sut.Execute(caseId, request);

			// Assert
			historyGatewayMock.Verify(h => h.CreateHistory(caseId, oldRatingId, "Old"), Times.Once);
			gatewayMock.Verify(g => g.Update(It.IsAny<ConcernsCase>()), Times.Once);
			Assert.NotNull(result);
		}
	}
}
