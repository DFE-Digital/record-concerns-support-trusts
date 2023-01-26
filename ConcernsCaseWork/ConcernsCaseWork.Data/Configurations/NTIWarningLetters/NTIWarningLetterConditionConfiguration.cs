using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterConditionConfiguration : IEntityTypeConfiguration<NTIWarningLetterCondition>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterCondition> builder)
	{
		builder.ToTable("NTIWarningLetterCondition", "concerns");
		
		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIWarningLetterCondition[]
			{
				new NTIWarningLetterCondition{ Id = 1, Name = "Trust financial plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 1, DisplayOrder = 1 },
				new NTIWarningLetterCondition{ Id = 2, Name = "Action plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 1  },
				new NTIWarningLetterCondition{ Id = 3, Name = "Lines of accountability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 2  },
				new NTIWarningLetterCondition{ Id = 4, Name = "Providing sufficient challenge", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 3  },
				new NTIWarningLetterCondition{ Id = 5, Name = "Scheme of delegation", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 4  },
				new NTIWarningLetterCondition{ Id = 6, Name = "Publishing requirements (compliance with)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 3, DisplayOrder = 1  },
				new NTIWarningLetterCondition{ Id = 7, Name = "Financial returns", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 4, DisplayOrder = 1  }
			});
	}
}