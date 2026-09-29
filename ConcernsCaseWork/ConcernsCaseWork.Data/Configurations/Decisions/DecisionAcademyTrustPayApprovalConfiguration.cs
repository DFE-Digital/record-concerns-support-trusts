using ConcernsCaseWork.Data.Models.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.Decisions;

public class DecisionAcademyTrustPayApprovalConfiguration : IEntityTypeConfiguration<DecisionAcademyTrustPayApproval>
{
	public void Configure(EntityTypeBuilder<DecisionAcademyTrustPayApproval> builder)
	{
		builder.ToTable("ConcernsDecisionAcademyTrustPayApproval", "concerns");
		builder.HasKey(x => x.Id);
		builder.HasData(
			Enum.GetValues(typeof(API.Contracts.Decisions.AcademyTrustPayApproval)).Cast<API.Contracts.Decisions.AcademyTrustPayApproval>()
				.Select(enm => new DecisionAcademyTrustPayApproval(enm) { Name = enm.ToString() }));
	}
}
