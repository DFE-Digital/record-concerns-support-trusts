using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsTypeConfiguration : IEntityTypeConfiguration<ConcernsType>
{
	public void Configure(EntityTypeBuilder<ConcernsType> builder)
	{
		builder.ToTable("ConcernsType", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CType");

		builder.HasData(
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.FinancialDeficit,
				Name = "Deficit",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.FinancialProjectedDeficit,
				Name = "Projected deficit",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.ForceMajeure,
				Name = "Force majeure",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.FinancialGovernance,
				Name = "Financial governance",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.FinancialViability,
				Name = "Viability",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.Irregularity,
				Name = "Irregularity",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.IrregularitySuspectedFraud,
				Name = "Suspected fraud",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.Compliance,
				Name = "Financial compliance",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.Safeguarding,
				Name = "Safeguarding non-compliance",
				Description = null,
				CreatedAt = new DateTime(2023, 1, 24),
				UpdatedAt = new DateTime(2023, 1, 24)
			},
			new ConcernsType 
			{
				Id = (int)API.Contracts.Concerns.ConcernType.GovernanceCapability,
				Name = "Governance capability",
				Description = null,
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.NonCompliance,
				Name = "Non-compliance",
				Description = null,
				CreatedAt = new DateTime(2023, 10, 30),
				UpdatedAt = new DateTime(2023, 10, 30)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.ActualProjectedDeficit,
				Name = "Actual and/or projected deficit",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.ActualProjectedCashShortfall,
				Name = "Actual and/or projected cash shortfall",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.TrustColure,
				Name = "Trust Closure",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.FinancialManagementCompliance,
				Name = "Financial management/ATH compliance",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.LateFinancialReturns,
				Name = "Late financial returns",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			},
			new ConcernsType
			{
				Id = (int)API.Contracts.Concerns.ConcernType.IrregularitySelfReportedFraud,
				Name = "Irregularity and/or self-reported fraud",
				Description = null,
				CreatedAt = new DateTime(2026, 2, 2),
				UpdatedAt = new DateTime(2026, 2, 2)
			}
		);
	}
}