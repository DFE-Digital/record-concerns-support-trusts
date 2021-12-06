using ConcernsCaseWork.Pages.Admin;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Service.Redis.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class EditRolePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<EditRolePageModel>>();

			var roles = RoleFactory.BuildListRoleEnum();
			
			mockRbacManager.Setup(r => r.GetUserRoles(It.IsAny<string>()))
				.ReturnsAsync(roles);
			
			var pageModel = SetupEditRolePageModel(mockRbacManager.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("username", "testing");
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.UserName, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.Not.Null);
			Assert.That(pageModel.Roles, Is.Not.Null);
			Assert.That(pageModel.UserRoles, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Empty);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Admin::EditRolePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.GetUserRoles(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_RouteData_IsNull_Return_Page()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<EditRolePageModel>>();

			var roles = RoleFactory.BuildListRoleEnum();
			
			mockRbacManager.Setup(r => r.GetUserRoles(It.IsAny<string>()))
				.ReturnsAsync(roles);
			
			var pageModel = SetupEditRolePageModel(mockRbacManager.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("username", "");
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.UserName, Is.Null);
			Assert.That(pageModel.PreviousUrl, Is.Null);
			Assert.That(pageModel.Roles, Is.Null);
			Assert.That(pageModel.UserRoles, Is.Null);
			Assert.That(pageModel.TempData, Is.Not.Empty);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Admin::EditRolePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.GetUserRoles(It.IsAny<string>()), Times.Never);
		}		
		
		[Test]
		public async Task WhenOnPostEditRole_Redirect_AdminPage()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<EditRolePageModel>>();
			
			mockRbacManager.Setup(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()));
			
			var pageModel = SetupEditRolePageModel(mockRbacManager.Object, mockLogger.Object);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "role", new StringValues("Admin,Leader") },
					{ "username", new StringValues("test.test") }
				});
			
			// act
			await pageModel.OnPostEditRole("https://returnto/thispage");
			
			// assert
			Assert.That(pageModel.UserName, Is.Null);
			Assert.That(pageModel.PreviousUrl, Is.Null);
			Assert.That(pageModel.Roles, Is.Null);
			Assert.That(pageModel.UserRoles, Is.Null);
			Assert.That(pageModel.TempData, Is.Empty);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Admin::EditRolePageModel::OnPostEditRole")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()), Times.Once);
		}		
		
		[Test]
		public async Task WhenOnPostEditRole_RequestForm_Missing_Return_ErrorOnPage()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<EditRolePageModel>>();
			
			mockRbacManager.Setup(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()));
			
			var pageModel = SetupEditRolePageModel(mockRbacManager.Object, mockLogger.Object);
			
			// act
			await pageModel.OnPostEditRole("https://returnto/thispage");
			
			// assert
			Assert.That(pageModel.UserName, Is.Null);
			Assert.That(pageModel.PreviousUrl, Is.Null);
			Assert.That(pageModel.Roles, Is.Null);
			Assert.That(pageModel.UserRoles, Is.Null);
			Assert.That(pageModel.TempData, Is.Not.Empty);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Admin::EditRolePageModel::OnPostEditRole")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()), Times.Never);
		}		
		
		[Test]
		public async Task WhenOnPostEditRole_RequestForm_Missing_Return_ReloadsPage()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<EditRolePageModel>>();
			
			var roles = RoleFactory.BuildListRoleEnum();
			
			mockRbacManager.Setup(r => r.GetUserRoles(It.IsAny<string>()))
				.ReturnsAsync(roles);
			mockRbacManager.Setup(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()));
			
			var pageModel = SetupEditRolePageModel(mockRbacManager.Object, mockLogger.Object);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "username", new StringValues("test.test") }
				});
			
			// act
			await pageModel.OnPostEditRole("https://returnto/thispage");
			
			// assert
			Assert.That(pageModel.UserName, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.Not.Null);
			Assert.That(pageModel.Roles, Is.Not.Null);
			Assert.That(pageModel.UserRoles, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Empty);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Admin::EditRolePageModel::OnPostEditRole")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockRbacManager.Verify(r => r.UpdateUserRoles(It.IsAny<string>(), It.IsAny<List<RoleEnum>>()), Times.Never);
			mockRbacManager.Verify(r => r.GetUserRoles(It.IsAny<string>()), Times.Once);
		}
		
		private static EditRolePageModel SetupEditRolePageModel(IRbacManager rbacManager, ILogger<EditRolePageModel> logger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditRolePageModel(rbacManager, logger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}