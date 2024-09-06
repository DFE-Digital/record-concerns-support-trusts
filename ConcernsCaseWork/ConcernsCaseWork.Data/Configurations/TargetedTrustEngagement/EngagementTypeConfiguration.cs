using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.TargetedTrustEngagement;

public class EngagementTypeConfiguration : IEntityTypeConfiguration<TargetedTrustEngagementType>
{
	public void Configure(EntityTypeBuilder<TargetedTrustEngagementType> builder)
	{
		builder.ToTable("TargetedTrustEngagementActivityMapping", "concerns");
		builder.HasKey(x => x.Id);
	}
}