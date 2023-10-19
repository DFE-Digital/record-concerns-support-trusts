using ConcernsCaseWork.Data.Models.Decisions;
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
			Enum.GetValues(typeof(API.Contracts.Decisions.DecisionStatus)).Cast<API.Contracts.Decisions.DecisionStatus>()
				.Select(enm => new DecisionStatus(enm) { Name = enm.ToString() }));
	}
}