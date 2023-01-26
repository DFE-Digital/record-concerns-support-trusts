using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveReasonMappingConfiguration : IEntityTypeConfiguration<NoticeToImproveReasonMapping>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveReasonMapping> builder)
	{
		builder.ToTable("NoticeToImproveReasonMapping", "concerns");

		builder.HasKey(e => e.Id);
		
		builder
			.HasOne(n => n.NoticeToImprove)
			.WithMany(n => n.NoticeToImproveReasonsMapping)
			.HasForeignKey(n => n.NoticeToImproveId);

		builder
			.HasOne(n => n.NoticeToImproveReason)
			.WithMany(n => n.NoticeToImproveReasonsMapping)
			.HasForeignKey(n => n.NoticeToImproveReasonId);
	}
}