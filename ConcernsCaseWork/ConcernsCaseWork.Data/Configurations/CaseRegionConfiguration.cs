using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Data.Configurations;

public class CaseRegionConfiguration : IEntityTypeConfiguration<CaseRegion>
{
	public void Configure(EntityTypeBuilder<CaseRegion> builder)
	{
		builder.ToTable("Region", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CRegion__C5B214360AF620234");

		builder.HasData(
			new CaseRegion
			{
				Id = Region.EastMidlands,
				Name = "East Midlands",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.EastOfEngland,
				Name = "East of England",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.London,
				Name = "London",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.NorthEast,
				Name = "North East",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.NorthWest,
				Name = "North West",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.SouthEast,
				Name = "South East",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.SouthWest,
				Name = "South West",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.WestMidlands,
				Name = "West Midlands",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			},
			new CaseRegion
			{
				Id = Region.YorkshireAndTheHumber,
				Name = "Yorkshire and The Humber",
				CreatedAt = new DateTime(2023, 10, 9),
				UpdatedAt = new DateTime(2023, 10, 9)
			}
		);
	}
}