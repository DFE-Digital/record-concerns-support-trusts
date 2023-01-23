using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeConfiguration : IEntityTypeConfiguration<Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome>
{
	public void Configure(EntityTypeBuilder<Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome> builder)
	{
		builder.ToTable("DecisionOutcome", "concerns");
		builder.HasKey(x => x.DecisionOutcomeId);
		builder.Property(x => x.TotalAmount).HasColumnType("money");
		builder.HasMany(x => x.BusinessAreasConsulted).WithOne();
	}
}