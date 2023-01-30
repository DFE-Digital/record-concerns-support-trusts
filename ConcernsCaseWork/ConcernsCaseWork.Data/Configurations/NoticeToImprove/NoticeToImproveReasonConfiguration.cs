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
				new NoticeToImproveReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 5, Name = "Non-compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 6, Name = "Non-compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
				new NoticeToImproveReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
			});
	}
}