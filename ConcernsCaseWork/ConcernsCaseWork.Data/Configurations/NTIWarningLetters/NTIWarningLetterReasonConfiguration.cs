using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterReasonConfiguration : IEntityTypeConfiguration<NTIWarningLetterReason>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterReason> builder)
	{
		builder.ToTable("NTIWarningLetterReason", "concerns");
		
		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIWarningLetterReason[]
			{
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.CashFlowProblems, 
					Name = "Cash flow problems", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.CumulativeDeficitActual, 
					Name = "Cumulative deficit (actual)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.CumulativeDeficitProjected, 
					Name = "Cumulative deficit (projected)", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.GovernanceConcerns, 
					Name = "Governance concerns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.NonComplianceWithAcademiesFinancialTrustHandbook, 
					Name = "Non-compliance with Academies Financial/Trust Handbook", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.NonComplianceWithFinancialReturns, 
					Name = "Non-compliance with financial returns", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.RiskOfInsolvency, 
					Name = "Risk of insolvency", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIWarningLetterReason
				{ 
					Id = (int)API.Contracts.NtiWarningLetter.NtiWarningLetterReason.Safeguarding, 
					Name = "Safeguarding", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				}
			});
	}
}