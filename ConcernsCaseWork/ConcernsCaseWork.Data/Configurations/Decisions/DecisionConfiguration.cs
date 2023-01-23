using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
	public void Configure(EntityTypeBuilder<Decision> builder)
	{
		builder.ToTable("ConcernsDecision", "concerns");
		builder.HasKey(x => x.DecisionId);
		builder.Property(x => x.TotalAmountRequested).HasColumnType("money");
		builder.HasMany(x => x.DecisionTypes).WithOne();
		builder.HasOne(x => x.Outcome);
	}
}