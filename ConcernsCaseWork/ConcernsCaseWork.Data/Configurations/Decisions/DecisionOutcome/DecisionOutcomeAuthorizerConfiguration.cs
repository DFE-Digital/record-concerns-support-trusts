using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;

public class DecisionOutcomeAuthorizerConfiguration : IEntityTypeConfiguration<DecisionOutcomeAuthorizer>
{
	public void Configure(EntityTypeBuilder<DecisionOutcomeAuthorizer> builder)
	{
		builder.ToTable("DecisionOutcomeAuthorizer", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum
				.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer))
				.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer>()
				.Select(enm => new DecisionOutcomeAuthorizer() { Id = enm, Name = enm.ToString() }));
	}
}