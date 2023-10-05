using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeConfiguration : IEntityTypeConfiguration<Models.Decisions.Outcome.DecisionOutcome>
{
	public void Configure(EntityTypeBuilder<Models.Decisions.Outcome.DecisionOutcome> builder)
	{
		builder.ToTable("DecisionOutcome", "concerns");
		builder.HasKey(x => x.DecisionOutcomeId);
		builder.Property(x => x.TotalAmount).HasColumnType("money");
		builder.HasMany(x => x.BusinessAreasConsulted).WithOne();
		builder.HasIndex(x => x.DecisionId).IsUnique();
	}
}