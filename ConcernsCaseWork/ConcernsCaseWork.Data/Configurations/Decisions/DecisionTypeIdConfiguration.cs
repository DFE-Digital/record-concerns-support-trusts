using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionTypeIdConfiguration : IEntityTypeConfiguration<DecisionTypeId>
{
	public void Configure(EntityTypeBuilder<DecisionTypeId> builder)
	{
		builder.ToTable("ConcernsDecisionTypeId", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum.GetValues(typeof(Enums.Concerns.DecisionType)).Cast<Enums.Concerns.DecisionType>()
				.Select(enm => new DecisionTypeId(enm, enm.ToString())));
	}
}