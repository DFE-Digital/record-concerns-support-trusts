using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.SRMA;

public class SRMAReasonConfiguration : IEntityTypeConfiguration<SRMAReason>
{
	public void Configure(EntityTypeBuilder<SRMAReason> builder)
	{
		builder.ToTable("SRMAReason", "concerns");

		builder.HasKey(e => e.Id);
		
		builder.HasData(
			Enum.GetValues(typeof(API.Contracts.Srma.SRMAReasonOffered)).Cast<API.Contracts.Srma.SRMAReasonOffered>()
				.Where(enm => enm != API.Contracts.Srma.SRMAReasonOffered.Unknown)
				.Select(enm => new SRMAStatus
				{
					Id = (int)enm,
					Name = enm.ToString(),
					CreatedAt = new DateTime(2022, 05, 06),
					UpdatedAt = new DateTime(2022, 05, 06)
				}));
	}
}