using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionFacilityAgreedConfiguration : IEntityTypeConfiguration<DecisionDrawdownFacilityAgreed>
{
	public void Configure(EntityTypeBuilder<DecisionDrawdownFacilityAgreed> builder)
	{
		builder.ToTable("ConcernsDecisionDrawdownFacilityAgreed", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum.GetValues(typeof(Enums.Concerns.DecisionDrawdownFacilityAgreed)).Cast<Enums.Concerns.DecisionDrawdownFacilityAgreed>()
				.Select(enm => new DecisionDrawdownFacilityAgreed(enm) { Name = enm.ToString() }));
	}
}