using ConcernsCaseWork.Middleware;
using ConcernsCaseWork.Services.PageHistory;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Middleware
{
	[Parallelizable(ParallelScope.All)]
	public class NavigationHistoryMiddlewareTests
	{
		[Test]
		public async Task WhenNoValuesSet_DoesNotThrowException()
		{
			// Arrange
			var ctxt = new DefaultHttpContext();
			var storageHandlerMock = Mock.Of<IPageHistoryStorageHandler>();
			var sut = new NavigationHistoryMiddleware(Mock.Of<RequestDelegate>());

			// Act
			await sut.Invoke(ctxt, storageHandlerMock);

			// Assert
			Assert.True(true); // Just checking we got here with no exceptions being thrown
		}
		
		[Test]
		[TestCase("/favicon.ico")]
		[TestCase("/error/404")]
		[TestCase("/Error/any")]
		[TestCase("/error")]
		[TestCase("/v2/")]
		[TestCase("/V2/")]
		[TestCase("/v2/some/")]
		[TestCase("/V2/SoMe/ApI")]
		public async Task WhenHistoryIsEmpty_AndRequestPathIsNotAPage_PageHistoryIsNotUpdated(string currentPage)
		{
			// Arrange
			var ctxt = new DefaultHttpContext();
			var storageHandlerMock = new Mock<IPageHistoryStorageHandler>();
			storageHandlerMock.Setup(s => s.GetPageHistory(ctxt)).Returns(Array.Empty<string>());
			
			ctxt.Request.Path = currentPage;
			
			var sut = new NavigationHistoryMiddleware(Mock.Of<RequestDelegate>());
			
			// Act
			await sut.Invoke(ctxt, storageHandlerMock.Object);
		
			// Assert
			storageHandlerMock.Verify(s => s.GetPageHistory(ctxt), Times.Once());
			storageHandlerMock.VerifyNoOtherCalls();
		}

		[Test]
		[TestCaseSource(nameof(PageHistoriesWithLinearJourney))]
		[TestCaseSource(nameof(PageHistoriesWithBackwardNavigation))]
		public async Task WhenMiddlewareInvoked_ShouldUpdatePageHistory(string currentPage, string[] currentHistory, string[] expectedHistory)
		{
			// Arrange
			var ctxt = new DefaultHttpContext();
			var storageHandlerMock = new Mock<IPageHistoryStorageHandler>();
			storageHandlerMock.Setup(s => s.GetPageHistory(ctxt)).Returns(currentHistory);
			
			ctxt.Request.Path = currentPage;
			
			var sut = new NavigationHistoryMiddleware(Mock.Of<RequestDelegate>());
			
			// Act
			await sut.Invoke(ctxt, storageHandlerMock.Object);
		
			// Assert
			storageHandlerMock.Verify(s => s.GetPageHistory(ctxt), Times.Once());
			storageHandlerMock.Verify(s => s.SetPageHistory(ctxt, expectedHistory), Times.Once);
			storageHandlerMock.VerifyNoOtherCalls();
		}
		
		[Test]
		[TestCaseSource(nameof(PageHistoriesWithDuplicateLastPage))]
		public async Task WhenMiddlewareInvoked_ShouldNotUpdatePageHistory(string currentPage, string[] currentHistory, string[] expectedHistory)
		{
			// Arrange
			var ctxt = new DefaultHttpContext();
			var storageHandlerMock = new Mock<IPageHistoryStorageHandler>();
			storageHandlerMock.Setup(s => s.GetPageHistory(ctxt)).Returns(currentHistory);
			
			ctxt.Request.Path = currentPage;
			
			var sut = new NavigationHistoryMiddleware(Mock.Of<RequestDelegate>());
			
			// Act
			await sut.Invoke(ctxt, storageHandlerMock.Object);
		
			// Assert
			storageHandlerMock.Verify(s => s.GetPageHistory(ctxt), Times.Once());
			storageHandlerMock.Verify(s => s.SetPageHistory(ctxt, expectedHistory), Times.Never);
			storageHandlerMock.VerifyNoOtherCalls();
		}

		private static IEnumerable<TestCaseData> PageHistoriesWithLinearJourney()
		{
			yield return new TestCaseData(
				"/case/1040472/closed", 
				Array.Empty<string>(),
				new []
				{
					"/case/1040472/closed"
				});
			
			yield return new TestCaseData(
				"/", 
				Array.Empty<string>(),
				new []
				{
					"/"
				});
						
			yield return new TestCaseData(
				"/%23some-anchor", 
				Array.Empty<string>(),
				new []
				{
					"/%23some-anchor"
				});
			
			yield return new TestCaseData(
				"/case/1040472/closed", 
				new []
				{
					"/case/closed"
				},
				new []
				{
					"/case/closed", 
					"/case/1040472/closed"
				});
	
			yield return new TestCaseData(
				"/case/1040472/management/action/financialplan/4/closed", 
				new []
				{
					"/case/closed", 
					"/case/1040472/closed"
				},
				new []
				{
					"/case/closed", 
					"/case/1040472/closed", 
					"/case/1040472/management/action/financialplan/4/closed"
				});
		}
		
		private static IEnumerable<TestCaseData> PageHistoriesWithBackwardNavigation()
		{
			yield return new TestCaseData(
				"/case/1040472/closed", 
				new []
				{
					"/case/closed", 
					"/case/1040472/closed", 
					"/case/1040472/management/action/financialplan/4/closed"
				},
				new []
				{
					"/case/closed", 
					"/case/1040472/closed"
				});
		}
		
				
		private static IEnumerable<TestCaseData> PageHistoriesWithDuplicateLastPage()
		{
			yield return new TestCaseData(
				"/case/1040472/closed", 
				new []
				{
					"/case/closed", 
					"/case/1040472/closed"
				},
				new []
				{
					"/case/closed", 
					"/case/1040472/closed"
				});
			
			yield return new TestCaseData(
				"/", 
				new []
				{
					"/"
				},
				new []
				{
					"/"
				});
		}
	}
}
