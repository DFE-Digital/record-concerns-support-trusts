using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class NTIWarningLetterFactoryTests
    {

        [Fact]
        public void CreateDBModel_ExpectedNTIWarningLetter_WhenCreateNTIWarningLetterRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                CaseUrn = 123,
                DateLetterSent = dtCreated,
                Notes = "Notes",
                StatusId = 1,
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                WarningLetterReasonsMapping = new List<int>() { 1, 3},
                WarningLetterConditionsMapping = new List<int>() { 1, 3},
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };


            var createNTIWarningLetter = Builder<CreateNTIWarningLetterRequest>
                .CreateNew()
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateLetterSent = details.DateLetterSent)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.WarningLetterReasonsMapping = details.WarningLetterReasonsMapping)
                .With(r => r.WarningLetterConditionsMapping = details.WarningLetterConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();


            var expectedNTIWarningLetter = new NTIWarningLetter
            {
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateLetterSent = details.DateLetterSent,
                StatusId = details.StatusId,
                WarningLetterReasonsMapping = details.WarningLetterReasonsMapping.Select(r => {
                    return new NTIWarningLetterReasonMapping()
                    {
                        NTIWarningLetterReasonId = r
                    };
                }).ToList(),
                WarningLetterConditionsMapping = details.WarningLetterConditionsMapping.Select(r => {
                    return new NTIWarningLetterConditionMapping()
                    {
                        NTIWarningLetterConditionId = r
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NTIWarningLetterFactory.CreateDBModel(createNTIWarningLetter);
            response.Should().BeEquivalentTo(expectedNTIWarningLetter);
        }

        [Fact]
        public void CreateDBModel_ExpectedNTIWarningLetter_WhenPatchTIWarningLetterRequestProvided()
        {
            var dtCreated = DateTime.Now;

            var details = new
            {
                Id = 111,
                CaseUrn = 123,
                DateLetterSent = dtCreated,
                Notes = "Notes",
                StatusId = 1,
                CreatedBy = "Test Name",
                CreatedAt = dtCreated,
                UpdatedAt = dtCreated,
                WarningLetterReasonsMapping = new List<int>() { 1, 3 },
                WarningLetterConditionsMapping = new List<int>() { 1, 3 },
                ClosedStatusId = 1,
                ClosedAt = dtCreated
            };


            var patchNTIWarningLetter = Builder<PatchNTIWarningLetterRequest>
                .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateLetterSent = details.DateLetterSent)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.WarningLetterReasonsMapping = details.WarningLetterReasonsMapping)
                .With(r => r.WarningLetterConditionsMapping = details.WarningLetterConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();


            var expectedNTIWarningLetter = new NTIWarningLetter
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateLetterSent = details.DateLetterSent,
                StatusId = details.StatusId,
                WarningLetterReasonsMapping = details.WarningLetterReasonsMapping.Select(r => {
                    return new NTIWarningLetterReasonMapping()
                    {
                        NTIWarningLetterReasonId = r
                    };
                }).ToList(),
                WarningLetterConditionsMapping = details.WarningLetterConditionsMapping.Select(r => {
                    return new NTIWarningLetterConditionMapping()
                    {
                        NTIWarningLetterConditionId = r
                    };
                }).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

            var response = NTIWarningLetterFactory.CreateDBModel(patchNTIWarningLetter);
            response.Should().BeEquivalentTo(expectedNTIWarningLetter);
        }


       [Fact]
       public void CreateResponse_ExpectedNTIWarningLetterResponse_WhenNTIWarningLetterProvided()
       {
           var dtCreated = DateTime.Now;

           var details = new
           {
               Id = 456,
               CaseUrn = 123,
               DateLetterSent = dtCreated,
               Notes = "Notes",
               StatusId = 1,
               CreatedBy = "Test Name",
               CreatedAt = dtCreated,
               UpdatedAt = dtCreated,
               WarningLetterReasonsMapping = new List<int>() { 1, 3 }.Select(r =>
               {
                   return new NTIWarningLetterReasonMapping() { NTIWarningLetterReasonId = r };
               }).ToList(),
               WarningLetterConditionsMapping = new List<int>() { 1, 3 }.Select(c =>
               {
                   return new NTIWarningLetterConditionMapping() { NTIWarningLetterConditionId = c };
               }).ToList(),
               ClosedStatusId = 1,
               ClosedAt = dtCreated
           };


           var ntiWarningLetter = Builder<NTIWarningLetter>
               .CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.DateLetterSent = details.DateLetterSent)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.WarningLetterReasonsMapping = details.WarningLetterReasonsMapping)
                .With(r => r.WarningLetterConditionsMapping = details.WarningLetterConditionsMapping)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .With(r => r.ClosedStatusId = details.ClosedStatusId)
                .With(r => r.ClosedAt = details.ClosedAt)
                .Build();

            var expectedNTIWarningLetterResponse = new NTIWarningLetterResponse
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Notes = details.Notes,
                DateLetterSent = details.DateLetterSent,
                StatusId = details.StatusId,
                WarningLetterReasonsMapping = details.WarningLetterReasonsMapping.Select(r => r.NTIWarningLetterReasonId).ToList(),
                WarningLetterConditionsMapping = details.WarningLetterConditionsMapping.Select(r => r.NTIWarningLetterConditionId).ToList(),
                CreatedBy = details.CreatedBy,
                CreatedAt = details.CreatedAt,
                UpdatedAt = details.UpdatedAt,
                ClosedStatusId = details.ClosedStatusId,
                ClosedAt = details.ClosedAt
            };

           var response = NTIWarningLetterFactory.CreateResponse(ntiWarningLetter);
           response.Should().BeEquivalentTo(expectedNTIWarningLetterResponse);
       }
    }
}
