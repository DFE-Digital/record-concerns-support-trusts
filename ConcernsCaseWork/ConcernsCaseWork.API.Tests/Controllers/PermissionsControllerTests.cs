using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
	public class PermissionsControllerTests
	{
		[Fact]
		public async Task GetPermissions_When_No_UserInfo_Returns_BadRequest()
		{
			/*
			 * 	ILogger<PermissionsController> logger,
				IServerUserInfoService userInfoService,
				IGetCasePermissionsUseCase getCasePermissionsUseCase)
			 */
			var mockLogger = new Mock<ILogger<PermissionsController>>();
			var mockUserInfoService = new Mock<IServerUserInfoService>();
			var mockUseCase = new Mock<IGetCasePermissionsUseCase>();

			var sut = new PermissionsController(mockLogger.Object, mockUserInfoService.Object, mockUseCase.Object);

			var request = new PermissionQueryRequest() { CaseIds = new long[] { 123 } };

			mockUserInfoService.Setup(x => x.UserInfo).Returns(default(UserInfo?));

			var response = await sut.GetPermissions(request, CancellationToken.None);

			response.Result.Should().BeAssignableTo<BadRequestObjectResult>();
			((BadRequestObjectResult)response.Result).Value.Should().Be("User information is null, cannot determined if current user owns cases or has permissions");
		}

		[Fact]
		public async Task GetPermissions_When_UserInfo_And_CaseIds_Returns_PermissionResults()
		{
			var mockLogger = new Mock<ILogger<PermissionsController>>();
			var mockUserInfoService = new Mock<IServerUserInfoService>();
			var mockUseCase = new Mock<IGetCasePermissionsUseCase>();

			UserInfo fakeUserInfo = new() { Name = "John.Smith", Roles = new[] { Claims.CaseWorkerRoleClaim } };
			mockUserInfoService.SetupGet(x => x.UserInfo).Returns(fakeUserInfo);

			var fakeCaseIds = new long[] { 123 };
			var fakePermissionResults = new[] { CasePermission.View, CasePermission.Edit };
			var fakePermissionQueryResponse = new PermissionQueryResponse()
			{
				CasePermissionResponses = new[] { new CasePermissionResponse() { CaseId = fakeCaseIds[0], Permissions = fakePermissionResults } }
			};

			mockUseCase.Setup(x => x.Execute(It.Is<(long[], UserInfo?)>(a => a.Item1[0] == fakeCaseIds[0] && a.Item2 == fakeUserInfo), It.IsAny<CancellationToken>()))
				.ReturnsAsync(fakePermissionQueryResponse);

			var sut = new PermissionsController(mockLogger.Object, mockUserInfoService.Object, mockUseCase.Object);

			// Act
			var fakePermissionsRequest = new PermissionQueryRequest() { CaseIds = fakeCaseIds };
			var response = await sut.GetPermissions(fakePermissionsRequest, CancellationToken.None);

			response.Result.Should().BeAssignableTo<OkObjectResult>();
			((OkObjectResult)response.Result).Value.Should().BeAssignableTo<ApiSingleResponseV2<PermissionQueryResponse>>();

			var dataBody = ((OkObjectResult)response.Result).Value as ApiSingleResponseV2<PermissionQueryResponse>;
			dataBody.Data.CasePermissionResponses[0].CaseId.Should().Be(fakeCaseIds[0]);
			dataBody.Data.CasePermissionResponses[0].Permissions.Should().BeEquivalentTo(fakePermissionResults);
		}
	}
}
