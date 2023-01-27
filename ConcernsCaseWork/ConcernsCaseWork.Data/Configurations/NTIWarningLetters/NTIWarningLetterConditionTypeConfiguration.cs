using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterConditionTypeConfiguration : IEntityTypeConfiguration<NTIWarningLetterConditionType>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterConditionType> builder)
	{
		builder.ToTable("NTIWarningLetterConditionType", "concerns");
		
		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIWarningLetterConditionType[]
			{
				new NTIWarningLetterConditionType{ Id = 1, Name = "Financial management conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 1 },
				new NTIWarningLetterConditionType{ Id = 2, Name = "Governance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 2 },
				new NTIWarningLetterConditionType{ Id = 3, Name = "Compliance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 3 },
				new NTIWarningLetterConditionType{ Id = 4, Name = "Standard conditions (mandatory)", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 4 }
			});
	}
}