using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsRatingConfiguration : IEntityTypeConfiguration<ConcernsRating>
{
	public void Configure(EntityTypeBuilder<ConcernsRating> builder)
	{
		builder.ToTable("ConcernsRating", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CRating");

		builder.HasData(
			new ConcernsRating
			{
				Id = 1,
				Name = "Red-Plus",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsRating
			{
				Id = 2,
				Name = "Red",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsRating
			{
				Id = 3,
				Name = "Red-Amber",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsRating
			{
				Id = 4,
				Name = "Amber-Green",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsRating
			{
				Id = 5,
				Name = "n/a",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			});
	}
}