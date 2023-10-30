using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveReasonConfiguration : IEntityTypeConfiguration<NoticeToImproveReason>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveReason> builder)
	{
		builder.ToTable("NoticeToImproveReason", "concerns");

		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 08, 17);

		builder.HasData(
			new NoticeToImproveReason[]
			{
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.CashFlowProblems, 
					Name = "Cash flow problems", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.CumulativeDeficitActual, 
					Name = "Cumulative deficit (actual)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.CumulativeDeficitProjected, 
					Name = "Cumulative deficit (projected)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.GovernanceConcerns, 
					Name = "Governance concerns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.NonComplianceWithAcademiesFinancialTrustHandbook, 
					Name = "Non-compliance with Academies Financial/Trust Handbook", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.NonComplianceWithFinancialReturns, 
					Name = "Non-compliance with financial returns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.RiskOfInsolvency, 
					Name = "Risk of insolvency", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NoticeToImproveReason
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiReason.Safeguarding, 
					Name = "Safeguarding", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				}
			});
	}
}