using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterConditionMappingConfiguration : IEntityTypeConfiguration<NTIWarningLetterConditionMapping>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterConditionMapping> builder)
	{
		builder.ToTable("NTIWarningLetterConditionMapping", "concerns");
		
		builder.HasKey(e => e.Id);
		
		builder
			.HasOne(n => n.NTIWarningLetter)
			.WithMany(n => n.WarningLetterConditionsMapping)
			.HasForeignKey(n => n.NTIWarningLetterId);

		builder
			.HasOne(n => n.NTIWarningLetterCondition)
			.WithMany(n => n.WarningLetterConditionsMapping)
			.HasForeignKey(n => n.NTIWarningLetterConditionId);
	}
}