using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeStatusConfiguration : IEntityTypeConfiguration<DecisionOutcomeStatus>
{
	public void Configure(EntityTypeBuilder<DecisionOutcomeStatus> builder)
	{
		builder.ToTable("DecisionOutcomeStatus", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum
				.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus))
				.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus>()
				.Select(enm => new DecisionOutcomeStatus() { Id = enm, Name = enm.ToString() }));
	}
}
