using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.SRMA;

public class SRMACaseConfiguration : IEntityTypeConfiguration<SRMACase>
{
	public void Configure(EntityTypeBuilder<SRMACase> builder)
	{
		builder.ToTable("SRMACase", "concerns");

		builder.HasKey(e => e.Id);
		
		builder.HasIndex(x => new {x.CaseUrn, x.CreatedAt}).IsUnique();
	}
}