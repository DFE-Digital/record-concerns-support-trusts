using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsRecordConfiguration : IEntityTypeConfiguration<ConcernsRecord>
{
	public void Configure(EntityTypeBuilder<ConcernsRecord> builder)
	{
		builder.ToTable("ConcernsRecord", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CRecord");
		
		builder.HasOne(r => r.ConcernsCase)
			.WithMany(c => c.ConcernsRecords)
			.HasForeignKey(r => r.CaseId)
			.HasConstraintName("FK__ConcernsCase_ConcernsRecord");

		builder.HasOne(r => r.ConcernsType)
			.WithMany(c => c.FkConcernsRecord)
			.HasForeignKey(r => r.TypeId)
			.HasConstraintName("FK__ConcernsRecord_ConcernsType");

		builder.HasOne(r => r.ConcernsRating)
			.WithMany(c => c.FkConcernsRecord)
			.HasForeignKey(r => r.RatingId)
			.HasConstraintName("FK__ConcernsRecord_ConcernsRating");

		builder.HasOne(e => e.ConcernsMeansOfReferral)
			.WithMany(e => e.FkConcernsRecord)
			.HasForeignKey(e => e.MeansOfReferralId)
			.HasConstraintName("FK__ConcernsRecord_ConcernsMeansOfReferral");
		
		builder.HasIndex(x => new {x.CaseId, x.CreatedAt}).IsUnique();
	}
}