using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Security;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class HomePageModelTests
	{
		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnCases()
		{
			// arrange
			var homeModels = HomePageFactory.BuildHomeModels();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();

			var roles = RoleFactory.BuildListRoleEnum();
			var defaultUsers = new[]{ "user1", "user2" };
			var roleClaimWrapper = new RoleClaimWrapper { Roles = roles, Users = defaultUsers };
			
			mockRbacManager.Setup(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()))
				.ReturnsAsync(roleClaimWrapper);
			mockCaseModelService.Setup(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(homeModels);
			
			var homePageModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object);
			
			// act
			await homePageModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(homePageModel.CasesActive);
			Assert.That(homePageModel.CasesActive.Count, Is.EqualTo(homeModels.Count));
			
			foreach (var expected in homePageModel.CasesActive)
			{
				foreach (var actual in homeModels.Where(actual => expected.CaseUrn.Equals(actual.CaseUrn)))
				{
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.CreatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Created));
					Assert.That(expected.Updated, Is.EqualTo(actual.Updated));
					Assert.That(DateTimeOffset.FromUnixTimeMilliseconds(expected.UpdatedUnixTime).ToString("dd-MM-yyyy"), Is.EqualTo(actual.Updated));
					Assert.That(expected.Review, Is.EqualTo(actual.Review));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.TrustNameTitle, Is.EqualTo(actual.TrustName.ToTitle()));

					var expectedRecordsModel = expected.RecordsModel;
					var actualRecordsModel = actual.RecordsModel;
					
					for (var index = 0; index < expectedRecordsModel.Count; ++index)
					{
						Assert.That(expectedRecordsModel.ElementAt(index).Urn, Is.EqualTo(actualRecordsModel.ElementAt(index).Urn));
						Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).CaseUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).RatingUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).RatingUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).StatusUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).StatusUrn));
						Assert.That(expectedRecordsModel.ElementAt(index).TypeUrn, Is.EqualTo(actualRecordsModel.ElementAt(index).TypeUrn));
						
						var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
						var actualRecordRatingModel = actualRecordsModel.ElementAt(index).RatingModel;
						Assert.NotNull(expectedRecordRatingModel);
						Assert.NotNull(actualRecordRatingModel);
						Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
						Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
						Assert.That(expectedRecordRatingModel.Urn, Is.EqualTo(actualRecordRatingModel.Urn));
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
					}
				}
			}
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()), Times.Once);
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()), Times.Once);
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()), Times.Exactly(1));
		}

		[Test]
		public async Task WhenInstanceOfIndexPageOnGetAsync_ReturnEmptyCases()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<HomePageModel>>();
			var emptyList = new List<HomeModel>();
			
			var roles = RoleFactory.BuildListUserRoleEnum();
			var defaultUsers = new[]{ "user1", "user2" };
			var roleClaimWrapper = new RoleClaimWrapper { Roles = roles, Users = defaultUsers };
			
			mockRbacManager.Setup(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()))
				.ReturnsAsync(roleClaimWrapper);
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);
			mockCaseModelService.Setup(model => model.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()))
				.ReturnsAsync(emptyList);
			
			// act
			var indexModel = SetupHomeModel(mockCaseModelService.Object, mockRbacManager.Object, mockLogger.Object);
			await indexModel.OnGetAsync();
			
			// assert
			Assert.IsAssignableFrom<List<HomeModel>>(indexModel.CasesActive);
			Assert.That(indexModel.CasesActive.Count, Is.Zero);
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("HomePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.GetUserRoleClaimWrapper(It.IsAny<string>()), Times.Once);
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string[]>(), It.IsAny<StatusEnum>()), Times.Never);
			mockCaseModelService.Verify(c => c.GetCasesByCaseworkerAndStatus(It.IsAny<string>(), It.IsAny<StatusEnum>()), Times.Exactly(1));
		}
		
		private static HomePageModel SetupHomeModel(ICaseModelService mockCaseModelService, IRbacManager mockRbacManager, ILogger<HomePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new HomePageModel(mockCaseModelService, mockRbacManager, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}