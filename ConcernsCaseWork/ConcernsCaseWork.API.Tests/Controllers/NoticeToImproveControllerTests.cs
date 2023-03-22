using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
	public class NoticeToImproveControllerTests
	{
		private readonly Mock<ILogger<NoticeToImproveController>> _mockLogger;
		private readonly Mock<IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse>> _mockCreateNoticeToImproveUseCase;
		private readonly Mock<IUseCase<long, NoticeToImproveResponse>> _mockGetNoticeToImproveByIdUseCase;
		private readonly Mock<IUseCase<int, List<NoticeToImproveResponse>>> _mockGetNoticeToImproveByCaseUrnUseCase;
		private readonly Mock<IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse>> _mockPatchNoticeToImproveUseCase;
		private readonly Mock<IUseCase<object, List<NoticeToImproveStatus>>> _mockGetAllStatuses;
		private readonly Mock<IUseCase<object, List<NoticeToImproveReason>>> _mockGetAllReasons;
		private readonly Mock<IUseCase<object, List<NoticeToImproveCondition>>> _mockGetAllCondition;
		private readonly Mock<IUseCase<object, List<NoticeToImproveConditionType>>> _mockGetAllConditionType;

		private readonly NoticeToImproveController controllerSUT;

		public NoticeToImproveControllerTests()
		{
			_mockLogger = new Mock<ILogger<NoticeToImproveController>>();
			_mockCreateNoticeToImproveUseCase = new Mock<IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse>>();
			_mockGetNoticeToImproveByIdUseCase = new Mock<IUseCase<long, NoticeToImproveResponse>>();
			_mockGetNoticeToImproveByCaseUrnUseCase = new Mock<IUseCase<int, List<NoticeToImproveResponse>>>();
			_mockPatchNoticeToImproveUseCase = new Mock<IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse>>();
			_mockGetAllStatuses = new Mock<IUseCase<object, List<NoticeToImproveStatus>>>();
			_mockGetAllReasons = new Mock<IUseCase<object, List<NoticeToImproveReason>>>();
			_mockGetAllCondition = new Mock<IUseCase<object, List<NoticeToImproveCondition>>>();
			_mockGetAllConditionType = new Mock<IUseCase<object, List<NoticeToImproveConditionType>>>();

			controllerSUT = new NoticeToImproveController(_mockLogger.Object, _mockCreateNoticeToImproveUseCase.Object, _mockGetNoticeToImproveByIdUseCase.Object,
				_mockGetNoticeToImproveByCaseUrnUseCase.Object, _mockPatchNoticeToImproveUseCase.Object, _mockGetAllStatuses.Object, _mockGetAllReasons.Object,
				_mockGetAllCondition.Object, _mockGetAllConditionType.Object);
		}

		[Fact]
		public async Task Create_ReturnsApiSingleResponseWithNewNoticeToImprove()
		{
			var createdAt = DateTime.Now;
			var caseUrn = 544;

			var response = Builder<NoticeToImproveResponse>
				.CreateNew()
				.With(r => r.CreatedAt = createdAt)
				.Build();

			var expectedResponse = new ApiSingleResponseV2<NoticeToImproveResponse>(response);

			_mockCreateNoticeToImproveUseCase
				.Setup(x => x.Execute(It.IsAny<CreateNoticeToImproveRequest>()))
				.Returns(response);

			var result = await controllerSUT.Create(new CreateNoticeToImproveRequest { CaseUrn = caseUrn, CreatedAt = createdAt });

			result.Result.Should().BeEquivalentTo(new ObjectResult(expectedResponse) { StatusCode = StatusCodes.Status201Created });
		}

		[Fact]
		public async Task GetAllStatuses_ReturnsAllStatuses()
		{
			var noOfStatuses = 5;

			var statuses = Builder<NoticeToImproveStatus>
				.CreateListOfSize(noOfStatuses)
				.Build()
				.ToList();

			_mockGetAllStatuses
				.Setup(x => x.Execute(null))
				.Returns(statuses);

			var controllerResponse = (await controllerSUT.GetAllStatuses()).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NoticeToImproveStatus>>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Count.Should().Be(noOfStatuses);
			actualResult.Data.First().Name.Should().Be(statuses.First().Name);
		}

		[Fact]
		public async Task GetAllReasons_ReturnsAllReasons()
		{
			var noOfReasons = 8;

			var reasons = Builder<NoticeToImproveReason>
				.CreateListOfSize(noOfReasons)
				.Build()
				.ToList();

			_mockGetAllReasons
				.Setup(x => x.Execute(null))
				.Returns(reasons);

			var controllerResponse = (await controllerSUT.GetAllReasons()).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NoticeToImproveReason>>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Count.Should().Be(noOfReasons);
			actualResult.Data.First().Name.Should().Be(reasons.First().Name);
		}

		[Fact]
		public async Task GetAllConditions_ReturnsAllConditions()
		{
			var noOfConditions = 7;

			var conditions = Builder<NoticeToImproveCondition>
				.CreateListOfSize(noOfConditions)
				.Build()
				.ToList();

			_mockGetAllCondition
				.Setup(x => x.Execute(null))
				.Returns(conditions);

			var controllerResponse = (await controllerSUT.GetAllConditions()).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NoticeToImproveCondition>>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Count.Should().Be(noOfConditions);
			actualResult.Data.First().Name.Should().Be(conditions.First().Name);
		}

		[Fact]
		public async Task GetAllConditionTypes_ReturnsAllConditionTypes()
		{
			var noOfConditionTypes = 4;

			var conditionTypes = Builder<NoticeToImproveConditionType>
				.CreateListOfSize(noOfConditionTypes)
				.Build()
				.ToList();

			_mockGetAllConditionType
				.Setup(x => x.Execute(null))
				.Returns(conditionTypes);

			var controllerResponse = (await controllerSUT.GetAllConditionTypes()).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NoticeToImproveConditionType>>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Count.Should().Be(noOfConditionTypes);
			actualResult.Data.First().Name.Should().Be(conditionTypes.First().Name);
		}

		[Fact]
		public async Task GetNoticeToImproveByCaseUrn_ReturnsMatchingNoticeToImprove_WhenGivenCaseUrn()
		{
			var caseUrn = 544;

			var matchingNoticeToImprove = new NoticeToImprove { CaseUrn = caseUrn, Notes = "Match" };

			var noticesToImprove = new List<NoticeToImprove>
			{
				matchingNoticeToImprove,
				new NoticeToImprove { CaseUrn = 123, Notes = "Notice To Improve 1" },
				new NoticeToImprove { CaseUrn = 456, Notes = "Notice To Improve 2" },
			};

			var noticeToImproveResponse = Builder<NoticeToImproveResponse>
				.CreateNew()
				.With(r => r.CaseUrn = matchingNoticeToImprove.CaseUrn)
				.With(r => r.Notes = matchingNoticeToImprove.Notes)
				.Build();

			var collection = new List<NoticeToImproveResponse> { noticeToImproveResponse };

			_mockGetNoticeToImproveByCaseUrnUseCase
				.Setup(x => x.Execute(caseUrn))
				.Returns(collection);

			var controllerResponse = (await controllerSUT.GetNoticesToImproveByCaseUrn(caseUrn)).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NoticeToImproveResponse>>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Count.Should().Be(1);
			actualResult.Data.First().CaseUrn.Should().Be(caseUrn);
		}

		[Fact]
		public async Task GetNoticeToImproveByID_ReturnsMatchingNoticeToImprove_WhenGivenId()
		{
			var noticeToImproveId = 455;

			var matchingNoticeToImprove = new NoticeToImprove { Id = noticeToImproveId, Notes = "Match" };

			var noticeToImproveResponse = Builder<NoticeToImproveResponse>
				.CreateNew()
				.With(r => r.Id = matchingNoticeToImprove.Id)
				.With(r => r.Notes = matchingNoticeToImprove.Notes)
				.Build();

			_mockGetNoticeToImproveByIdUseCase
				.Setup(x => x.Execute(noticeToImproveId))
				.Returns(noticeToImproveResponse);


			var controllerResponse = (await controllerSUT.GetNoticeToImproveById(noticeToImproveId)).Result as OkObjectResult;


			var actualResult = controllerResponse.Value as ApiSingleResponseV2<NoticeToImproveResponse>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Id.Should().Be(noticeToImproveId);
		}

		[Fact]
		public async Task PatchNoticeToImprove_ReturnsUpdatedNoticeToImprove()
		{
			var noticeToImproveId = 544;
			var newNotes = "updated notes";
			var newClosedStatusId = 2;

			var originalNTI = new NoticeToImprove() { Id = noticeToImproveId, Notes = "original note" };

			var request = Builder<PatchNoticeToImproveRequest>
				.CreateNew()
				.With(r => r.Id = originalNTI.Id)
				.With(r => r.Notes = newNotes)
				.With(r => r.ClosedStatusId = newClosedStatusId)
				.Build();

			var response = Builder<NoticeToImproveResponse>
				.CreateNew()
				.With(r => r.Id = request.Id)
				.With(r => r.Notes = request.Notes)
				.With(r => r.ClosedStatusId = request.ClosedStatusId)
				.Build();

			_mockPatchNoticeToImproveUseCase
				.Setup(x => x.Execute(request))
				.Returns(response);

			var controllerResponse = (await controllerSUT.Patch(request)).Result as OkObjectResult;

			var actualResult = controllerResponse.Value as ApiSingleResponseV2<NoticeToImproveResponse>;

			actualResult.Data.Should().NotBeNull();
			actualResult.Data.Id.Should().Be(noticeToImproveId);
			actualResult.Data.Notes.Should().Be(newNotes);
			actualResult.Data.ClosedStatusId.Should().Be(newClosedStatusId);
		}
	}
}