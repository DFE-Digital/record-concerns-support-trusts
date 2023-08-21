using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsCaseConfiguration : IEntityTypeConfiguration<ConcernsCase>
{
	public void Configure(EntityTypeBuilder<ConcernsCase> builder)
	{
		builder.ToTable("ConcernsCase", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CCase__C5B214360AF620234");
		
		builder.Property(e => e.Urn)
			.HasComputedColumnSql("[Id]");

		builder.Property(e => e.CreatedBy).HasMaxLength(254);

		builder.HasMany(x => x.Decisions)
			.WithOne();

		builder.Property(e => e.Territory)
			.HasConversion<string>();

		builder.Property(e => e.TrustUkprn).HasMaxLength(12);
		
		builder.HasIndex(x => new {x.TrustUkprn, x.CreatedAt, x.CreatedBy}).IsUnique();

		builder.Property(x => x.TrustCompaniesHouseNumber)
			.HasMaxLength(8);

		builder.HasQueryFilter(f => !f.DeletedAt.HasValue);

	}
}