using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterConfiguration : IEntityTypeConfiguration<NTIWarningLetter>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetter> builder)
	{
		builder.ToTable("NTIWarningLetterCase", "concerns");
		
		builder.HasKey(e => e.Id);
		
		builder.HasIndex(x => new {x.CaseUrn, x.CreatedAt}).IsUnique();
	}
}