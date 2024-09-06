using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.TargetedTrustEngagement;

public class TargetedTrustEngagementOutcomeConfiguration : IEntityTypeConfiguration<TargetedTrustEngagementOutcome>
{
	public void Configure(EntityTypeBuilder<TargetedTrustEngagementOutcome> builder)
	{
		builder.ToTable("TargetedTrustEngagementOutcomeType", "concerns");

		builder.HasKey(e => e.Id);
    
		builder.HasData(
    		Enum.GetValues(typeof(API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementOutcome)).Cast<API.Contracts.TargetedTrustEngagement.TargetedTrustEngagementOutcome>()
    			.Select(enm => new TargetedTrustEngagementOutcome
				{
    				Id = (int)enm,
    				Name = enm.ToString(),
    				CreatedAt = new DateTime(2024, 08, 27),
    				UpdatedAt = new DateTime(2024, 08, 27)
    			}));
	}
}