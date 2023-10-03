using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsTypeConfiguration : IEntityTypeConfiguration<ConcernsType>
{
	public void Configure(EntityTypeBuilder<ConcernsType> builder)
	{
		builder.ToTable("ConcernsType", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CType");

		builder.HasData(
			new ConcernsType
			{
				Id = 3,
				Name = "Deficit",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 4,
				Name = "Projected deficit",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 7,
				Name = "Force majeure",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 8,
				Name = "Financial governance",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 20,
				Name = "Viability",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 21,
				Name = "Irregularity",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 22,
				Name = "Suspected fraud",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 23,
				Name = "Financial compliance",
				Description = null,
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2023, 10, 03)
			},
			new ConcernsType
			{
				Id = 24,
				Name = "Safeguarding",
				Description = null,
				CreatedAt = new DateTime(2023, 1, 24),
				UpdatedAt = new DateTime(2023, 1, 24)
			}
		);
	}
}