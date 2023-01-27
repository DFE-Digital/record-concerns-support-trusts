using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveStatusConfiguration : IEntityTypeConfiguration<NoticeToImproveStatus>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveStatus> builder)
	{
		builder.ToTable("NoticeToImproveStatus", "concerns");

		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 08, 17);

		builder.HasData(
			new NoticeToImproveStatus[]
			{
				new NoticeToImproveStatus{ Id = 1, Name = "Preparing NTI", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 2, Name = "Issued NTI", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 3, Name = "Progress on track", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false},
				new NoticeToImproveStatus{ Id = 4, Name = "Evidence of NTI non-compliance", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 5, Name = "Serious NTI breaches - considering escalation to TWN", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 6, Name = "Submission to lift NTI in progress", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 7, Name = "Submission to close NTI in progress", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
				new NoticeToImproveStatus{ Id = 8, Name = "Lifted", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
				new NoticeToImproveStatus{ Id = 9, Name = "Closed", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
				new NoticeToImproveStatus{ Id = 10, Name = "Cancelled", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true }
			});
	}
}