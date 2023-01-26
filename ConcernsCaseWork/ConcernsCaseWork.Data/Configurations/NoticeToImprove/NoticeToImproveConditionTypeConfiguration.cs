using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveConditionTypeConfiguration : IEntityTypeConfiguration<NoticeToImproveConditionType>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveConditionType> builder)
	{
		builder.ToTable("NoticeToImproveConditionType", "concerns");

		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NoticeToImproveConditionType[]
			{
				new NoticeToImproveConditionType{ Id = 1, Name = "Financial management conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 1 },
				new NoticeToImproveConditionType{ Id = 2, Name = "Governance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 2 },
				new NoticeToImproveConditionType{ Id = 3, Name = "Compliance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 3 },
				new NoticeToImproveConditionType{ Id = 4, Name = "Safeguarding conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 4 },
				new NoticeToImproveConditionType{ Id = 5, Name = "Fraud and irregularity", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 5 },
				new NoticeToImproveConditionType{ Id = 6, Name = "Standard conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 6 },
				new NoticeToImproveConditionType{ Id = 7, Name = "Additional Financial Support conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 7 }
			});
	}
}