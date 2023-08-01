using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionFrameworkCategoryConfiguration : IEntityTypeConfiguration<DecisionFrameworkCategory>
{
	public void Configure(EntityTypeBuilder<DecisionFrameworkCategory> builder)
	{
		builder.ToTable("ConcernsDecisionFrameworkCategory", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum.GetValues(typeof(API.Contracts.Decisions.FrameworkCategory)).Cast<API.Contracts.Decisions.FrameworkCategory>()
				.Select(enm => new DecisionFrameworkCategory(enm) { Name = enm.ToString() }));
	}
}