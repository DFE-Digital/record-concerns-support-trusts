using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class ClosedPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Returns_ClosedCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();

			var homeModels = HomePageFactory.BuildHomeModels();
			
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(homeModels);
			
			var pageModel = SetupClosedPageModel(mockCaseModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			var homeModel = pageModel.CasesClosed;
			
			// assert
			Assert.That(homeModel, Is.Not.Null);
			Assert.IsAssignableFrom<List<HomeModel>>(homeModel);
			Assert.That(homeModel.Count, Is.EqualTo(homeModels.Count));
			
			foreach (var expected in homeModel)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(expected.Review, Is.EqualTo(actual.Review));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));

					var expectedRecordsModel = expected.RecordsModel;
					var actualRecordsModel = actual.RecordsModel;
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Id, Is.EqualTo(actualRecordsModel.ElementAt(index).Id));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingId, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingId));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusId, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusId));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeId, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeId));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Id, Is.EqualTo(actualRecordRatingModel.Id));
						Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
						Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));
						
						var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
						var actualRecordTypeModel = actualRecordsModel.ElementAt(index).TypeModel;
						Assert.NotNull(expectedRecordTypeModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
						Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
						Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
						Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

						var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
						var actualRecordStatusModel = actualRecordsModel.ElementAt(index).StatusModel;
						Assert.NotNull(expectedRecordStatusModel);
						Assert.NotNull(actualRecordTypeModel);
						Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
						Assert.That(expectedRecordStatusModel.Urn, Is.EqualTo(actualRecordStatusModel.Urn));
					}
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosedPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<ClosedPageModel>>();
			
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.Throws<Exception>();

			var pageModel = SetupClosedPageModel(mockCaseModelService.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();
			var homeModel = pageModel.CasesClosed;

			// assert
			Assert.That(homeModel, Is.Not.Null);
			Assert.IsAssignableFrom<HomeModel[]>(homeModel);
			Assert.That(homeModel.Count, Is.EqualTo(0));
		}

		private static ClosedPageModel SetupClosedPageModel(
			ICaseModelService mockCaseModelService, ILogger<ClosedPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosedPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}