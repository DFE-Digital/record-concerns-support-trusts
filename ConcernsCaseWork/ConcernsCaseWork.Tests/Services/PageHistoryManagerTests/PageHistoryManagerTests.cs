using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.PageHistory;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.PageHistoryManagerTests
{
	[Parallelizable(ParallelScope.All)]
	public class PageHistoryManagerTests
	{
		[Test]
		[TestCase("/favicon.ico")]
		[TestCase("/error/404")]
		[TestCase("/Error/any")]
		[TestCase("/error")]
		[TestCase("/v2/")]
		[TestCase("/V2/")]
		[TestCase("/v2/some/")]
		[TestCase("/V2/SoMe/ApI")]
		public void WhenHistoryIsEmpty_AndRequestPathIsNotAPage_PageHistoryIsNotUpdated(string currentPage)
		{
			var pageHistory = new List<string>();
		
			var updatedPageHistory = PageHistoryManager.BuildPageHistory(currentPage, pageHistory);
		
			updatedPageHistory.Should().BeEmpty();
		}
		
		[Test]
		[TestCaseSource(nameof(PageHistoriesWithLinearJourney))]
		public void WhenPageHistoryIsLinearJourney_ShouldUpdatePageHistory(string currentPage, string[] pageHistory, string[] expectedHistory)
		{
			var updatedPageHistory = PageHistoryManager.BuildPageHistory(currentPage, pageHistory.ToList());
		
			updatedPageHistory.Should().Equal(expectedHistory);
		}
				
		[Test]
		[TestCaseSource(nameof(PageHistoriesWithBackwardNavigation))]
		public void WhenPageHistoryHasBackwardNavigation_ShouldUpdatePageHistory(string currentPage, string[] pageHistory, string[] expectedHistory)
		{
			var updatedPageHistory = PageHistoryManager.BuildPageHistory(currentPage, pageHistory);
		
			updatedPageHistory.Should().Equal(expectedHistory);
		}
		
		[Test]
		[TestCaseSource(nameof(PageHistoriesWithDuplicateLastPage))]
		public void WhenLastPageIsDuplicateOfCurrentPage_ShouldNotUpdatePageHistory(string currentPage, string[] pageHistory, string[] expectedHistory)
		{
			var updatedPageHistory =PageHistoryManager.BuildPageHistory(currentPage, pageHistory);
			
			updatedPageHistory.Should().Equal(expectedHistory);
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
			
			yield return new TestCaseData(
				"/pageB", 
				new []
				{
					"/pagea", 
					"/pageb", 
					"/pagec"
				},
				new []
				{
					"/pagea", 
					"/pageb"
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