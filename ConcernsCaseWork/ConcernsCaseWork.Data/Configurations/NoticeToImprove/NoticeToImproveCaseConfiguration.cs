using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveCaseConfiguration : IEntityTypeConfiguration<Models.NoticeToImprove>
{
	public void Configure(EntityTypeBuilder<Models.NoticeToImprove> builder)
	{
		builder.ToTable("NoticeToImproveCase", "concerns");

		builder.HasKey(e => e.Id);
		
		builder.HasIndex(x => new {x.CaseUrn, x.CreatedAt}).IsUnique();
	}
}