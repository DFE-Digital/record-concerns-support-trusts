using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveConditionMappingConfiguration : IEntityTypeConfiguration<NoticeToImproveConditionMapping>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveConditionMapping> builder)
	{
		builder.ToTable("NoticeToImproveConditionMapping", "concerns");

		builder.HasKey(e => e.Id);

		builder
			.HasOne(n => n.NoticeToImprove)
			.WithMany(n => n.NoticeToImproveConditionsMapping)
			.HasForeignKey(n => n.NoticeToImproveId);

		builder
			.HasOne(n => n.NoticeToImproveCondition)
			.WithMany(n => n.NoticeToImproveConditionsMapping)
			.HasForeignKey(n => n.NoticeToImproveConditionId);
	}
}