using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.SRMA;

public class SRMAStatusConfiguration : IEntityTypeConfiguration<SRMAStatus>
{
	public void Configure(EntityTypeBuilder<SRMAStatus> builder)
	{
		builder.ToTable("SRMAStatus", "concerns");

		builder.HasKey(e => e.Id);
    
		builder.HasData(
    		Enum.GetValues(typeof(API.Contracts.Srma.SRMAStatus)).Cast<API.Contracts.Srma.SRMAStatus>()
    			.Where(enm => enm != API.Contracts.Srma.SRMAStatus.Unknown)
    			.Select(enm => new SRMAStatus
    			{
    				Id = (int)enm,
    				Name = enm.ToString(),
    				CreatedAt = new DateTime(2022, 05, 06),
    				UpdatedAt = new DateTime(2022, 05, 06)
    			}));
	}
}