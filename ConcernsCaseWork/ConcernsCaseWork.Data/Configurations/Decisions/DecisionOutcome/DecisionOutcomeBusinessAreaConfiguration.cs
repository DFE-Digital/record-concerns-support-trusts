using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeBusinessAreaConfiguration : IEntityTypeConfiguration<DecisionOutcomeBusinessArea>
{
	public void Configure(EntityTypeBuilder<DecisionOutcomeBusinessArea> builder)
	{
		builder.ToTable("DecisionOutcomeBusinessArea", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum
				.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea))
				.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea>()
				.Select(enm => new DecisionOutcomeBusinessArea() { Id = enm, Name = enm.ToString() }));
	}
}