using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
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
    public class NTIUnderConsiderationControllerTests
    {
        private readonly Mock<ILogger<NTIUnderConsiderationController>> _mockLogger;
        private readonly Mock<IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>> _mockCreateNtiUnderConsiderationUseCase;
        private readonly Mock<IUseCase<long, NTIUnderConsiderationResponse>> _mockGetNtiUnderConsiderationByIdUseCase;
        private readonly Mock<IUseCase<int, List<NTIUnderConsiderationResponse>>> _mockGetNtiUnderConsiderationByCaseUrnUseCase;
        private readonly Mock<IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>> _mockPatchNTIUnderConsiderationUseCase;
        private readonly Mock<IUseCase<object, List<NTIUnderConsiderationStatus>>> _mockGetAllStatuses;
        private readonly Mock<IUseCase<object, List<NTIUnderConsiderationReason>>> _mockGetAllReasons;

        private readonly NTIUnderConsiderationController controllerSUT;

        public NTIUnderConsiderationControllerTests()
        {
            _mockLogger = new Mock<ILogger<NTIUnderConsiderationController>>();
            _mockCreateNtiUnderConsiderationUseCase = new Mock<IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>>();
            _mockGetNtiUnderConsiderationByIdUseCase = new Mock<IUseCase<long, NTIUnderConsiderationResponse>>();
            _mockGetNtiUnderConsiderationByCaseUrnUseCase = new Mock<IUseCase<int, List<NTIUnderConsiderationResponse>>>();
            _mockPatchNTIUnderConsiderationUseCase = new Mock<IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>>();
            _mockGetAllStatuses = new Mock<IUseCase<object, List<NTIUnderConsiderationStatus>>>();
            _mockGetAllReasons = new Mock<IUseCase<object, List<NTIUnderConsiderationReason>>>();

            controllerSUT = new NTIUnderConsiderationController(_mockLogger.Object, _mockCreateNtiUnderConsiderationUseCase.Object, _mockGetNtiUnderConsiderationByIdUseCase.Object,
                _mockGetNtiUnderConsiderationByCaseUrnUseCase.Object, _mockPatchNTIUnderConsiderationUseCase.Object, _mockGetAllStatuses.Object, _mockGetAllReasons.Object);
        }

        [Fact]
        public async Task Create_ReturnsApiSingleResponseWithNewNTIUnderConsideration()
        {
            var createdAt = DateTime.Now;
            var caseUrn = 544;

            var response = Builder<NTIUnderConsiderationResponse>
               .CreateNew()
               .With(r => r.CreatedAt = createdAt)
               .Build();

            var expectedResponse = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(response);

            _mockCreateNtiUnderConsiderationUseCase
                .Setup(x => x.Execute(It.IsAny<CreateNTIUnderConsiderationRequest>()))
                .Returns(response);

            var result = await controllerSUT.Create(new CreateNTIUnderConsiderationRequest
            {
                CaseUrn = caseUrn,
                CreatedAt = createdAt
            });

            result.Result.Should().BeEquivalentTo(new ObjectResult(expectedResponse) { StatusCode = StatusCodes.Status201Created });
        }

        [Fact]
        public async Task GetAllStatuses_ReturnsAllStatuses()
        {
            var noOfStatuses = 2;

            var statuses = Builder<NTIUnderConsiderationStatus>
                .CreateListOfSize(noOfStatuses)
                .Build()
                .ToList();

            _mockGetAllStatuses
                .Setup(x => x.Execute(null))
                .Returns(statuses);

            // todo: chris review
            var controllerResponse = (await controllerSUT.GetAllStatuses()).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIUnderConsiderationStatus>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfStatuses);
            actualResult.Data.First().Name.Should().Be(statuses.First().Name);
        }

        [Fact]
        public async Task GetAllReasons_ReturnsAllReasons()
        {
            var noOfReasons = 8;

            var reasons = Builder<NTIUnderConsiderationReason>
                .CreateListOfSize(noOfReasons)
                .Build()
                .ToList();

            _mockGetAllReasons
                .Setup(x => x.Execute(null))
                .Returns(reasons);

            // todo: chris review
            var controllerResponse = (await controllerSUT.GetAllReasons()).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIUnderConsiderationReason>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfReasons);
            actualResult.Data.First().Name.Should().Be(reasons.First().Name);
        }

        [Fact]
        public async Task GetNTIUnderConsiderationByCaseUrn_ReturnsMatchingNTIUnderConsideration_WhenGivenCaseUrn()
        {
            var caseUrn = 544;

            var matchingConsideration = new NTIUnderConsideration
            {
                CaseUrn = caseUrn,
                Notes = "Match"
            };

            var considerations = new List<NTIUnderConsideration>
            {
                matchingConsideration,
                new NTIUnderConsideration
                {
                    CaseUrn = 123,
                    Notes = "Consideration 1"
                },
                new NTIUnderConsideration
                {
                    CaseUrn = 456,
                    Notes = "Consideration 2"
                },
            };

            var considerationResponse = Builder<NTIUnderConsiderationResponse>
              .CreateNew()
              .With(r => r.CaseUrn = matchingConsideration.CaseUrn)
              .With(r => r.Notes = matchingConsideration.Notes)
              .Build();

            var collection = new List<NTIUnderConsiderationResponse> { considerationResponse };

            _mockGetNtiUnderConsiderationByCaseUrnUseCase
                .Setup(x => x.Execute(caseUrn))
                .Returns(collection);

			// todo: chris review
			var controllerResponse = (await controllerSUT.GetNtiUnderConsiderationByCaseUrn(caseUrn)).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIUnderConsiderationResponse>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(1);
            actualResult.Data.First().CaseUrn.Should().Be(caseUrn);
        }

        [Fact]
        public async Task GetNTIUnderConsiderationByID_ReturnsMatchingNTIUnderConsideration_WhenGivenId()
        {
            var considerationId = 455;

            var matchingConsideration = new NTIUnderConsideration
            {
                Id = considerationId,
                Notes = "Match"
            };

            var considerationResponse = Builder<NTIUnderConsiderationResponse>
              .CreateNew()
              .With(r => r.Id = matchingConsideration.Id)
              .With(r => r.Notes = matchingConsideration.Notes)
              .Build();

            _mockGetNtiUnderConsiderationByIdUseCase
                .Setup(x => x.Execute(considerationId))
                .Returns(considerationResponse);

            // todo: chris review
            var controllerResponse = (await controllerSUT.GetNTIUnderConsiderationById(considerationId)).Result as OkObjectResult;


            var actualResult = controllerResponse.Value as ApiSingleResponseV2<NTIUnderConsiderationResponse>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Id.Should().Be(considerationId);
        }

        [Fact]
        public async Task PatchNTIUnderConsideration_ReturnsUpdatedNTIUnderConsideration()
        {
            var underConsiderationId = 544;
            var newNotes = "updated notes";
            var newClosedStatusId = 2;

            var originalConsideration = new NTIUnderConsideration()
            {
                Id = underConsiderationId,
                Notes = "original note"
            };

            var request = Builder<PatchNTIUnderConsiderationRequest>
                .CreateNew()
                .With(r => r.Id = originalConsideration.Id)
                .With(r => r.Notes = newNotes)
                .With(r => r.ClosedStatusId = newClosedStatusId)
                .Build();

            var response = Builder<NTIUnderConsiderationResponse>
                .CreateNew()
                .With(r => r.Id = request.Id)
                .With(r => r.Notes = request.Notes)
                .With(r => r.ClosedStatusId = request.ClosedStatusId)
                .Build();

            _mockPatchNTIUnderConsiderationUseCase
                .Setup(x => x.Execute(request))
                .Returns(response);

            var controllerResponse = (await controllerSUT.Patch(request)).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<NTIUnderConsiderationResponse>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Id.Should().Be(underConsiderationId);
            actualResult.Data.Notes.Should().Be(newNotes);
            actualResult.Data.ClosedStatusId.Should().Be(newClosedStatusId);
        }
    }
}
