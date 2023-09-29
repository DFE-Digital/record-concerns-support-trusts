using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Data.Configurations;

public class CaseDivisionConfiguration : IEntityTypeConfiguration<CaseDivision>
{
	public void Configure(EntityTypeBuilder<CaseDivision> builder)
	{
		builder.ToTable("Division", "concerns");
		
		builder.HasKey(e => e.Id)
			.HasName("PK__CDivision__C5B214360AF620234");

		builder.HasData(
			new CaseDivision
			{
				Id = Division.SFSO,
				Name = "SFSO (Schools Financial Support and Oversight)",
				CreatedAt = new DateTime(2023, 9, 27),
				UpdatedAt = new DateTime(2023, 9, 27)
			},
			new CaseDivision
			{
				Id = Division.RegionsGroup,
				Name = "Regions Group",
				CreatedAt = new DateTime(2023, 9, 27),
				UpdatedAt = new DateTime(2023, 9, 27)
			}
		);
	}
}