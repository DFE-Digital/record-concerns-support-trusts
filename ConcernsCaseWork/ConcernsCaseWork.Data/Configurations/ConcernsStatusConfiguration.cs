using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsStatusConfiguration : IEntityTypeConfiguration<ConcernsStatus>
{
	public void Configure(EntityTypeBuilder<ConcernsStatus> builder)
	{
		builder.ToTable("ConcernsStatus", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CStatus__C5B214360AF620234");

		builder.HasData(
			new ConcernsStatus
			{
				Id = 1,
				Name = "Live",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsStatus
			{
				Id = 2,
				Name = "Monitoring",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			},
			new ConcernsStatus
			{
				Id = 3,
				Name = "Close",
				CreatedAt = new DateTime(2021, 11, 17),
				UpdatedAt = new DateTime(2021, 11, 17)
			}
		);
	}
}