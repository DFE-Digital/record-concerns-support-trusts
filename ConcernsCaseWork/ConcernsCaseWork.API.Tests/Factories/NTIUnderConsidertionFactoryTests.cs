using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class NTIUnderConsidertionFactoryTests
    {
        [Fact]
        public void CreateDBModel_ExpectedNTIUnderConsideration_WhenCreateNTIUnderConsiderationRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                CaseUrn = 123,
                Notes = "Notes",
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                UnderConsiderationReasonsMapping = new List<int>() { 1, 3},
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };

            var createNTIUnderConsideration = Builder<CreateNTIUnderConsiderationRequest>
                .CreateNew()
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Notes = details.Notes)
                .With(r => r.UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();

            var expectedNTIUnderConsideration = new NTIUnderConsideration
            {
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping.Select(r => {
                    return new NTIUnderConsiderationReasonMapping()
                    {
                        NTIUnderConsiderationReasonId = r
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NTIUnderConsiderationFactory.CreateDBModel(createNTIUnderConsideration);
            response.Should().BeEquivalentTo(expectedNTIUnderConsideration);
        }

        [Fact]
        public void CreateDBModel_ExpectedNTIUnderConsideration_WhenPatchTIUnderConsiderationRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                Id = 111,
                CaseUrn = 123,
                Notes = "Notes",
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                UnderConsiderationReasonsMapping = new List<int>() { 1, 3 },
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };

            var patchNTIUnderConsideration = Builder<PatchNTIUnderConsiderationRequest>
                .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Notes = details.Notes)
                .With(r => r.UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();

            var expectedNTIUnderConsideration = new NTIUnderConsideration
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping.Select(r => {
                    return new NTIUnderConsiderationReasonMapping()
                    {
                        NTIUnderConsiderationReasonId = r
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NTIUnderConsiderationFactory.CreateDBModel(patchNTIUnderConsideration);
            response.Should().BeEquivalentTo(expectedNTIUnderConsideration);
        }

        [Fact]
        public void CreateResponse_ExpectedNTIUnderConsiderationResponse_WhenNTIUnderConsiderationProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                Id = 456,
                CaseUrn = 123,
                Notes = "Notes",
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                UnderConsiderationReasonsMapping = new List<int>() { 1, 3 }.Select(r =>
                {
                    return new NTIUnderConsiderationReasonMapping() { NTIUnderConsiderationReasonId = r };
                }).ToList(),
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };

            var ntiUnderConsideration = Builder<NTIUnderConsideration>
                .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Notes = details.Notes)
                .With(r => r.UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();

            var expectedNTIUnderConsiderationResponse = new NTIUnderConsiderationResponse
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                UnderConsiderationReasonsMapping = details.UnderConsiderationReasonsMapping.Select(r => r.NTIUnderConsiderationReasonId).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NTIUnderConsiderationFactory.CreateResponse(ntiUnderConsideration);
            response.Should().BeEquivalentTo(expectedNTIUnderConsiderationResponse);
        }
    }
}
