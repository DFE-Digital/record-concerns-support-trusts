using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsMeansOfReferralConfiguration : IEntityTypeConfiguration<ConcernsMeansOfReferral>
{
	public void Configure(EntityTypeBuilder<ConcernsMeansOfReferral> builder)
	{
		builder.ToTable("ConcernsMeansOfReferral", "concerns");

		builder.HasKey(e => e.Id)
			.HasName("PK__CMeansOfReferral");

		builder.HasData(
			new ConcernsMeansOfReferral()
			{
				Id = 1,
				Name = "Internal",
				Description = "ESFA activity, TFF or other departmental activity",
				CreatedAt = new DateTime(2022, 7, 28),
				UpdatedAt = new DateTime(2023, 1, 27)
			},
			new ConcernsMeansOfReferral()
			{
				Id = 2,
				Name = "External",
				Description = "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies",
				CreatedAt = new DateTime(2022, 7, 28),
				UpdatedAt = new DateTime(2022, 11, 09)
			});
	}
}