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
				Name = "Financial",
				Description = "Deficit",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsType
			{
				Id = 4,
				Name = "Financial",
				Description = "Projected deficit",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsType
			{
				Id = 7,
				Name = "Force majeure",
				Description = null,
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsType
			{
				Id = 8,
				Name = "Governance and compliance",
				Description = "Governance",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsType
			{
				Id = 20,
				Name = "Financial",
				Description = "Viability",
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2022, 12, 20)
			},
			new ConcernsType
			{
				Id = 21,
				Name = "Irregularity",
				Description = "Irregularity",
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2022, 12, 20)
			},
			new ConcernsType
			{
				Id = 22,
				Name = "Irregularity",
				Description = "Suspected fraud",
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2022, 12, 20)
			},
			new ConcernsType
			{
				Id = 23,
				Name = "Governance and compliance",
				Description = "Compliance",
				CreatedAt = new DateTime(2022, 12, 20),
				UpdatedAt = new DateTime(2022, 12, 20)
			}
		);
	}
}