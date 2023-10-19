using ConcernsCaseWork.Data.Models.Decisions;
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
			Enum.GetValues(typeof(API.Contracts.Decisions.DecisionType)).Cast<API.Contracts.Decisions.DecisionType>()
				.Select(enm => new DecisionTypeId(enm, enm.ToString())));
	}
}