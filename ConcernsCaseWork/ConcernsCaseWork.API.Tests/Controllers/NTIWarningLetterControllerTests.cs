using ConcernsCaseWork.API.Controllers;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
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
    public class NTIWarningLetterControllerTests
    {
        private readonly Mock<ILogger<NTIWarningLetterController>> _mockLogger;
        private readonly Mock<IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse>> _mockCreateNtiWarningLetterUseCase;
        private readonly Mock<IUseCase<long, NTIWarningLetterResponse>> _mockGetNtiWarningLetterByIdUseCase;
        private readonly Mock<IUseCase<int, List<NTIWarningLetterResponse>>> _mockGetNtiWarningLetterByCaseUrnUseCase;
        private readonly Mock<IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse>> _mockPatchNTIWarningLetterUseCase;
        private readonly Mock<IUseCase<object, List<NTIWarningLetterStatus>>> _mockGetAllStatuses;
        private readonly Mock<IUseCase<object, List<NTIWarningLetterReason>>> _mockGetAllReasons;
        private readonly Mock<IUseCase<object, List<NTIWarningLetterCondition>>> _mockGetAllCondition;
        private readonly Mock<IUseCase<object, List<NTIWarningLetterConditionType>>> _mockGetAllConditionType;

        private readonly NTIWarningLetterController controllerSUT;

        public NTIWarningLetterControllerTests()
        {
            _mockLogger = new Mock<ILogger<NTIWarningLetterController>>();
            _mockCreateNtiWarningLetterUseCase = new Mock<IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse>>();
            _mockGetNtiWarningLetterByIdUseCase = new Mock<IUseCase<long, NTIWarningLetterResponse>>();
            _mockGetNtiWarningLetterByCaseUrnUseCase = new Mock<IUseCase<int, List<NTIWarningLetterResponse>>>();
            _mockPatchNTIWarningLetterUseCase = new Mock<IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse>>();
            _mockGetAllStatuses = new Mock<IUseCase<object, List<NTIWarningLetterStatus>>>();
            _mockGetAllReasons = new Mock<IUseCase<object, List<NTIWarningLetterReason>>>();
            _mockGetAllCondition = new Mock<IUseCase<object, List<NTIWarningLetterCondition>>>();
            _mockGetAllConditionType = new Mock<IUseCase<object, List<NTIWarningLetterConditionType>>>();

            controllerSUT = new NTIWarningLetterController(_mockLogger.Object, _mockCreateNtiWarningLetterUseCase.Object, _mockGetNtiWarningLetterByIdUseCase.Object,
                _mockGetNtiWarningLetterByCaseUrnUseCase.Object, _mockPatchNTIWarningLetterUseCase.Object, _mockGetAllStatuses.Object, _mockGetAllReasons.Object, _mockGetAllCondition.Object, _mockGetAllConditionType.Object);
        }

        [Fact]
        public void Create_ReturnsApiSingleResponseWithNewNTIWarningLetter()
        {
            var createdAt = DateTime.Now;
            var caseUrn = 544;

            var response = Builder<NTIWarningLetterResponse>
               .CreateNew()
               .With(r => r.CreatedAt = createdAt)
               .Build();

            var expectedResponse = new ApiSingleResponseV2<NTIWarningLetterResponse>(response);

            _mockCreateNtiWarningLetterUseCase
                .Setup(x => x.Execute(It.IsAny<CreateNTIWarningLetterRequest>()))
                .Returns(response);

            var result = controllerSUT.Create(new CreateNTIWarningLetterRequest
            {
                CaseUrn = caseUrn,
                CreatedAt = createdAt
            });

            result.Result.Should().BeEquivalentTo(new ObjectResult(expectedResponse) { StatusCode = StatusCodes.Status201Created });
        }

        [Fact]
        public async Task GetAllStatuses_ReturnsAllStatuses()
        {
            var noOfStatuses = 5;

            var statuses = Builder<NTIWarningLetterStatus>
                .CreateListOfSize(noOfStatuses)
                .Build()
                .ToList();

            _mockGetAllStatuses
                .Setup(x => x.Execute(null))
                .Returns(statuses);

            // todo: chris review
            var controllerResponse = await controllerSUT.GetAllStatuses(); //.Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIWarningLetterStatus>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfStatuses);
            actualResult.Data.First().Name.Should().Be(statuses.First().Name);
        }

        [Fact]
        public async Task GetAllReasons_ReturnsAllReasons()
        {
            var noOfReasons = 8;

            var reasons = Builder<NTIWarningLetterReason>
                .CreateListOfSize(noOfReasons)
                .Build()
                .ToList();

            _mockGetAllReasons
                .Setup(x => x.Execute(null))
                .Returns(reasons);

            // todo: chris review
            var controllerResponse = await controllerSUT.GetAllReasons(); //.Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIWarningLetterReason>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfReasons);
            actualResult.Data.First().Name.Should().Be(reasons.First().Name);
        }

        [Fact]
        public void GetAllConditions_ReturnsAllConditions()
        {
            var noOfConditions = 7;

            var conditions = Builder<NTIWarningLetterCondition>
                .CreateListOfSize(noOfConditions)
                .Build()
                .ToList();

            _mockGetAllCondition
                .Setup(x => x.Execute(null))
                .Returns(conditions);

            OkObjectResult controllerResponse = controllerSUT.GetAllConditions().Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIWarningLetterCondition>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfConditions);
            actualResult.Data.First().Name.Should().Be(conditions.First().Name);
        }

        [Fact]
        public void GetAllConditionTypes_ReturnsAllConditionTypes()
        {
            var noOfConditionTypes = 4;

            var conditionTypes = Builder<NTIWarningLetterConditionType>
                .CreateListOfSize(noOfConditionTypes)
                .Build()
                .ToList();

            _mockGetAllConditionType
                .Setup(x => x.Execute(null))
                .Returns(conditionTypes);

            OkObjectResult controllerResponse = controllerSUT.GetAllConditionTypes().Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIWarningLetterConditionType>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(noOfConditionTypes);
            actualResult.Data.First().Name.Should().Be(conditionTypes.First().Name);
        }

        [Fact]
        public void GetNTIWarningLetterByCaseUrn_ReturnsMatchingNTIWarningLetter_WhenGivenCaseUrn()
        {
            var caseUrn = 544;

            var matchingWarningLetter = new NTIWarningLetter
            {
                CaseUrn = caseUrn,
                Notes = "Match"
            };

            var considerations = new List<NTIWarningLetter>
            {
                matchingWarningLetter,
                new NTIWarningLetter
                {
                    CaseUrn = 123,
                    Notes = "Warning Letter 1"
                },
                new NTIWarningLetter
                {
                    CaseUrn = 456,
                    Notes = "Warning Letter 2"
                },
            };

            var considerationResponse = Builder<NTIWarningLetterResponse>
              .CreateNew()
              .With(r => r.CaseUrn = matchingWarningLetter.CaseUrn)
              .With(r => r.Notes = matchingWarningLetter.Notes)
              .Build();

            var collection = new List<NTIWarningLetterResponse> { considerationResponse };

            _mockGetNtiWarningLetterByCaseUrnUseCase
                .Setup(x => x.Execute(caseUrn))
                .Returns(collection);

            OkObjectResult controllerResponse = controllerSUT.GetNtiWarningLetterByCaseUrn(caseUrn).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<List<NTIWarningLetterResponse>>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Count.Should().Be(1);
            actualResult.Data.First().CaseUrn.Should().Be(caseUrn);
        }

        [Fact]
        public void GetNTIWarningLetterByID_ReturnsMatchingNTIWarningLetter_WhenGivenId()
        {
            var warningLetterId = 455;

            var matchingWarningLetter = new NTIWarningLetter
            {
                Id = warningLetterId,
                Notes = "Match"
            };

            var warningLetterResponse = Builder<NTIWarningLetterResponse>
              .CreateNew()
              .With(r => r.Id = matchingWarningLetter.Id)
              .With(r => r.Notes = matchingWarningLetter.Notes)
              .Build();

            _mockGetNtiWarningLetterByIdUseCase
                .Setup(x => x.Execute(warningLetterId))
                .Returns(warningLetterResponse);

            OkObjectResult controllerResponse = controllerSUT.GetNTIWarningLetterById(warningLetterId).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<NTIWarningLetterResponse>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Id.Should().Be(warningLetterId);
        }

        [Fact]
        public void PatchNTIWarningLetter_ReturnsUpdatedNTIWarningLetter()
        {
            var warningLetterId = 544;
            var newNotes = "updated notes";
            var newClosedStatusId = 2;

            var originalWarningLetter = new NTIWarningLetter()
            {
                Id = warningLetterId,
                Notes = "original note"
            };

            var request = Builder<PatchNTIWarningLetterRequest>
                .CreateNew()
                .With(r => r.Id = originalWarningLetter.Id)
                .With(r => r.Notes = newNotes)
                .With(r => r.ClosedStatusId = newClosedStatusId)
                .Build();

            var response = Builder<NTIWarningLetterResponse>
                .CreateNew()
                .With(r => r.Id = request.Id)
                .With(r => r.Notes = request.Notes)
                .With(r => r.ClosedStatusId = request.ClosedStatusId)
                .Build();

            _mockPatchNTIWarningLetterUseCase
                .Setup(x => x.Execute(request))
                .Returns(response);

            OkObjectResult controllerResponse = controllerSUT.Patch(request).Result as OkObjectResult;

            var actualResult = controllerResponse.Value as ApiSingleResponseV2<NTIWarningLetterResponse>;

            actualResult.Data.Should().NotBeNull();
            actualResult.Data.Id.Should().Be(warningLetterId);
            actualResult.Data.Notes.Should().Be(newNotes);
            actualResult.Data.ClosedStatusId.Should().Be(newClosedStatusId);
        }
    }
}
