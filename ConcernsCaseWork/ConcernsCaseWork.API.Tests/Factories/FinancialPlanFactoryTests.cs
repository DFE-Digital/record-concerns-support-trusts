using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Factories
{
    public class FinancialPlanFactoryTests
    {
        [Fact]
        public void CreateDBModel_ExpectedFinancialPlanCase_WhenCreateFinancialPlanRequestProvided()
        {
            var dtNow = DateTime.Now;
            var dtCreated = dtNow.AddDays(-2);

            var details = new
            {
                CaseUrn = 132,
                Name = "Test name",
                ClosedAt = dtNow,
                CreatedAt = dtCreated,
                CreatedBy = "Test user name",
                DatePlanRequested = dtCreated,
                DateViablePlanReceived = dtCreated,
                Notes = "Test notes abc",
                StatusId = 1,
                UpdatedAt = dtCreated

            };

            var createFinancialPlanRequest = Builder<CreateFinancialPlanRequest>.CreateNew()
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Name = details.Name)
                .With(r => r.ClosedAt = details.ClosedAt)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.DatePlanRequested = details.DatePlanRequested)
                .With(r => r.DateViablePlanReceived = details.DateViablePlanReceived)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .Build();

            var expectedFinancialPlan = new FinancialPlanCase
            {
                CaseUrn = details.CaseUrn,
                Name = details.Name,
                ClosedAt = details.ClosedAt,
                CreatedAt = details.CreatedAt,
                CreatedBy = details.CreatedBy,
                DatePlanRequested = details.DatePlanRequested,
                DateViablePlanReceived = details.DateViablePlanReceived,
                Notes = details.Notes,
                StatusId = details.StatusId,
                UpdatedAt = details.UpdatedAt
            };

            var response = FinancialPlanFactory.CreateDBModel(createFinancialPlanRequest);

            response.Should().BeEquivalentTo(expectedFinancialPlan);
        }

        [Fact]
        public void CreateDBModel_ExpectedFinancialPlanCase_WhenPatchFinancialPlanRequestProvided()
        {
            var dtNow = DateTime.Now;
            var dtCreated = dtNow.AddDays(-2);

            var details = new
            {
                Id = 789,
                CaseUrn = 132,
                Name = "Test name",
                ClosedAt = dtNow,
                CreatedAt = dtCreated,
                CreatedBy = "Test user name",
                DatePlanRequested = dtCreated,
                DateViablePlanReceived = dtCreated,
                Notes = "Test notes abc",
                StatusId = 1,
                UpdatedAt = dtCreated

            };

            var patchFinancialPlanRequest = Builder<PatchFinancialPlanRequest>.CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Name = details.Name)
                .With(r => r.ClosedAt = details.ClosedAt)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.DatePlanRequested = details.DatePlanRequested)
                .With(r => r.DateViablePlanReceived = details.DateViablePlanReceived)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .Build();

            var expectedFinancialPlan = new FinancialPlanCase
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Name = details.Name,
                ClosedAt = details.ClosedAt,
                CreatedAt = details.CreatedAt,
                CreatedBy = details.CreatedBy,
                DatePlanRequested = details.DatePlanRequested,
                DateViablePlanReceived = details.DateViablePlanReceived,
                Notes = details.Notes,
                StatusId = details.StatusId,
                UpdatedAt = details.UpdatedAt
            };

            var response = FinancialPlanFactory.CreateDBModel(patchFinancialPlanRequest);

            response.Should().BeEquivalentTo(expectedFinancialPlan);
        }

        [Fact]
        public void CreateCreateResponse_ExpectedFinancialPlanResponse_WhenFinancialPlanCaseProvided()
        {
            var dtNow = DateTime.Now;
            var dtCreated = dtNow.AddDays(-2);

            var details = new
            {
                Id = 789,
                CaseUrn = 132,
                Name = "Test name",
                ClosedAt = dtNow,
                CreatedAt = dtCreated,
                CreatedBy = "Test user name",
                DatePlanRequested = dtCreated,
                DateViablePlanReceived = dtCreated,
                Notes = "Test notes abc",
                StatusId = 1,
                UpdatedAt = dtCreated

            };

            var financialPlanCase = Builder<FinancialPlanCase>.CreateNew()
                .With(r => r.Id = details.Id)
                .With(r => r.CaseUrn = details.CaseUrn)
                .With(r => r.Name = details.Name)
                .With(r => r.ClosedAt = details.ClosedAt)
                .With(r => r.CreatedAt = details.CreatedAt)
                .With(r => r.CreatedBy = details.CreatedBy)
                .With(r => r.DatePlanRequested = details.DatePlanRequested)
                .With(r => r.DateViablePlanReceived = details.DateViablePlanReceived)
                .With(r => r.Notes = details.Notes)
                .With(r => r.StatusId = details.StatusId)
                .With(r => r.UpdatedAt = details.UpdatedAt)
                .Build();

            var expectedFinancialPlanResponse = new FinancialPlanResponse
            {
                Id = details.Id,
                CaseUrn = details.CaseUrn,
                Name = details.Name,
                ClosedAt = details.ClosedAt,
                CreatedAt = details.CreatedAt,
                CreatedBy = details.CreatedBy,
                DatePlanRequested = details.DatePlanRequested,
                DateViablePlanReceived = details.DateViablePlanReceived,
                Notes = details.Notes,
                StatusId = details.StatusId,
                UpdatedAt = details.UpdatedAt
            };

            var response = FinancialPlanFactory.CreateResponse(financialPlanCase);

            response.Should().BeEquivalentTo(expectedFinancialPlanResponse);
        }


    }
}
