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
				new NTIUnderConsiderationReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 5, Name = "Non-Compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 6, Name = "Non-Compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NTIUnderConsiderationReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
			});
	}
}