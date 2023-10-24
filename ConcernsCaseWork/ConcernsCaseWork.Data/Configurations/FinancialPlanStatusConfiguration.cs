using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class FinancialPlanStatusConfiguration : IEntityTypeConfiguration<FinancialPlanStatus>
{
	public void Configure(EntityTypeBuilder<FinancialPlanStatus> builder)
	{
		builder.ToTable("FinancialPlanStatus", "concerns");

		builder.HasKey(e => e.Id);

		var createdAt = new DateTime(2022, 06, 15);

		builder.HasData(
			new FinancialPlanStatus[]
			{
				new FinancialPlanStatus
				{
					Id = (int)API.Contracts.FinancialPlan.FinancialPlanStatus.AwaitingPlan, 
					Name = "AwaitingPlan",
					Description = "Awaiting Plan", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosedStatus = false 
				},
				new FinancialPlanStatus
				{ 
					Id = (int)API.Contracts.FinancialPlan.FinancialPlanStatus.ReturnToTrust, 
					Name = "ReturnToTrust", 
					Description = "Return To Trust", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosedStatus = false 
				},
				new FinancialPlanStatus
				{ 
					Id = (int)API.Contracts.FinancialPlan.FinancialPlanStatus.ViablePlanReceived, 
					Name = "ViablePlanReceived", 
					Description = "Viable Plan Received", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosedStatus = true 
				},
				new FinancialPlanStatus
				{ 
					Id = (int)API.Contracts.FinancialPlan.FinancialPlanStatus.Abandoned, 
					Name = "Abandoned", 
					Description = "Abandoned", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosedStatus = true 
				}
			});
	}
}
