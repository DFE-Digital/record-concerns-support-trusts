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
				new NTIWarningLetterReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 5, Name = "Non-compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 6, Name = "Non-compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIWarningLetterReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
			});
	}
}