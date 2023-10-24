using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NtiUnderConsideration;

public class NTIUnderConsiderationReasonConfiguration : IEntityTypeConfiguration<NTIUnderConsiderationReason>
{
	public void Configure(EntityTypeBuilder<NTIUnderConsiderationReason> builder)
	{
		builder.ToTable("NTIUnderConsiderationReason", "concerns");
		
		builder.HasKey(e => e.Id);

		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIUnderConsiderationReason[]
			{
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.CashFlowProblems, 
					Name = "Cash flow problems", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.CumulativeDeficitActual, 
					Name = "Cumulative deficit (actual)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.CumulativeDeficitProjected, 
					Name = "Cumulative deficit (projected)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.GovernanceConcerns, 
					Name = "Governance concerns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.NonComplianceWithAcademiesFinancialTrustHandbook, 
					Name = "Non-Compliance with Academies Financial/Trust Handbook", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.NonComplianceWithFinancialReturns, 
					Name = "Non-Compliance with financial returns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.RiskOfInsolvency,
					Name = "Risk of insolvency", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationReason
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationReason.Safeguarding, 
					Name = "Safeguarding", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				}
			});
	}
}