using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.TargetedTrustEngagement;

public class TargetedTrustEngagementActivityConfiguration : IEntityTypeConfiguration<TargetedTrustEngagementActivity>
{
	public void Configure(EntityTypeBuilder<TargetedTrustEngagementActivity> builder)
	{
		builder.ToTable("TargetedTrustEngagementActivity", "concerns");

		builder.HasKey(e => e.Id);
    
		builder.HasData(
    		Enum.GetValues(typeof(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity)).Cast<API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity>()
    			.Select(enm => new TargetedTrustEngagementActivity
				{
    				Id = (int)enm,
    				Name = enm.ToString(),
    				CreatedAt = new DateTime(2024, 08, 27),
    				UpdatedAt = new DateTime(2024, 08, 27)
    			}));
	}
}