using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterReasonMappingConfiguration : IEntityTypeConfiguration<NTIWarningLetterReasonMapping>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterReasonMapping> builder)
	{
		builder.ToTable("NTIWarningLetterReasonMapping", "concerns");
		
		builder.HasKey(e => e.Id);
		
		builder
			.HasOne(n => n.NTIWarningLetter)
			.WithMany(n => n.WarningLetterReasonsMapping)
			.HasForeignKey(n => n.NTIWarningLetterId);

		builder
			.HasOne(n => n.NTIWarningLetterReason)
			.WithMany(n => n.WarningLetterReasonsMapping)
			.HasForeignKey(n => n.NTIWarningLetterReasonId);
	}
}