using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NtiUnderConsideration;

public class NTIUnderConsiderationCaseConfiguration : IEntityTypeConfiguration<NTIUnderConsideration>
{
	public void Configure(EntityTypeBuilder<NTIUnderConsideration> builder)
	{
		builder.ToTable("NTIUnderConsiderationCase", "concerns");
		
		builder.HasKey(e => e.Id);

		builder.HasOne(x => x.ClosedStatus);
		
		builder.HasIndex(x => new {x.CaseUrn, x.CreatedAt}).IsUnique();
	}
}