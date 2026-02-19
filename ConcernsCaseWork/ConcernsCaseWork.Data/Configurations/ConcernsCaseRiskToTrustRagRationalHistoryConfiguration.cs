using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations
{
    public class ConcernsCaseRiskToTrustRagRationalHistoryConfiguration : IEntityTypeConfiguration<ConcernsCaseRiskToTrustRatingHistory>
	{
		public void Configure(EntityTypeBuilder<ConcernsCaseRiskToTrustRatingHistory> builder)
		{
			builder.ToTable("ConcernsCaseRiskToTrustRatingHistory", "concerns");

			builder.HasKey(e => e.Id)
				.HasName("PK__RiskToTrustRatingHistory");


			builder.Property(x => x.CaseId)
				.IsRequired();

			builder.Property(x => x.RatingId)
				.IsRequired();

			builder.Property(x => x.RationalCommentary)
				.IsRequired()
				.HasMaxLength(250);

			builder.HasIndex(e => e.CaseId);
			builder.HasIndex(e => e.CreatedAt);
		}
	}
}
