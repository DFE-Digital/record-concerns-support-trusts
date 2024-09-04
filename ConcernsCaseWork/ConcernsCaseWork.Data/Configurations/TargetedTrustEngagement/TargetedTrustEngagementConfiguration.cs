using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.TargetedTrustEngagement;

public class TargetedTrustEngagementConfiguration : IEntityTypeConfiguration<TargetedTrustEngagementCase>
{
	public void Configure(EntityTypeBuilder<TargetedTrustEngagementCase> builder)
	{
		builder.ToTable("TargetedTrustEngagementCase", "concerns");

		builder.HasKey(e => e.Id);
		
		builder.HasIndex(x => new {x.CaseUrn, x.CreatedAt}).IsUnique();

		builder.HasQueryFilter(f => !f.DeletedAt.HasValue);
	}
}