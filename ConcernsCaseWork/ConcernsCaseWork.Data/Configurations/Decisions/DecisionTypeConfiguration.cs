using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionTypeConfiguration : IEntityTypeConfiguration<DecisionType>
{
	public void Configure(EntityTypeBuilder<DecisionType> builder)
	{
		builder.ToTable("ConcernsDecisionType", "concerns");
		builder.HasKey(x => new { x.DecisionId, x.DecisionTypeId });
	}
}