using ConcernsCaseWork.Data.Models.Decisions.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeBusinessAreaMappingConfiguration : IEntityTypeConfiguration<DecisionOutcomeBusinessAreaMapping>
{
	public void Configure(EntityTypeBuilder<DecisionOutcomeBusinessAreaMapping> builder)
	{
		builder.ToTable("DecisionOutcomeBusinessAreaMapping", "concerns");
		builder.HasKey(x => new { x.DecisionOutcomeId, x.DecisionOutcomeBusinessId });
	}
}