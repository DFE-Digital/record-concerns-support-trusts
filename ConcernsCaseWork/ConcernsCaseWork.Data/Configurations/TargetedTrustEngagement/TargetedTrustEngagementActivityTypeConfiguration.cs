using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.TargetedTrustEngagement;

public class TargetedTrustEngagementActivityTypeConfiguration : IEntityTypeConfiguration<TargetedTrustEngagementActivityType>
{
	public void Configure(EntityTypeBuilder<TargetedTrustEngagementActivityType> builder)
	{
		builder.ToTable("TargetedTrustEngagementActivityType", "concerns");

		builder.HasKey(e => e.Id);
    
		builder.HasData(
    		Enum.GetValues(typeof(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType)).Cast<API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType>()
    			.Select(enm => new TargetedTrustEngagementActivityType
				{
    				Id = (int)enm,
    				Name = enm.ToString(),
    				CreatedAt = new DateTime(2024, 08, 27),
    				UpdatedAt = new DateTime(2024, 08, 27)
    			}));
	}
}