using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionStatusConfiguration : IEntityTypeConfiguration<DecisionStatus>
{
	public void Configure(EntityTypeBuilder<DecisionStatus> builder)
	{
		builder.ToTable("ConcernsDecisionStates", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum.GetValues(typeof(Enums.Concerns.DecisionStatus)).Cast<Enums.Concerns.DecisionStatus>()
				.Select(enm => new DecisionStatus(enm) { Name = enm.ToString() }));
	}
}