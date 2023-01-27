using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NTIWarningLetters;

public class NTIWarningLetterStatusConfiguration : IEntityTypeConfiguration<NTIWarningLetterStatus>
{
	public void Configure(EntityTypeBuilder<NTIWarningLetterStatus> builder)
	{
		builder.ToTable("NTIWarningLetterStatus", "concerns");
		
		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIWarningLetterStatus[]
			{
				new NTIWarningLetterStatus{ Id = 1, Name = "Preparing warning letter", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false, PastTenseName="" },
				new NTIWarningLetterStatus{ Id = 2, Name = "Sent to trust", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false, PastTenseName="" },
				new NTIWarningLetterStatus{ Id = 3, Name = "Cancel warning letter", Description="The warning letter is no longer needed.", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Cancelled" },
				new NTIWarningLetterStatus{ Id = 4, Name = "Conditions met", Description="You are satisfied that all the conditions have been, or will be, met as outlined in the letter", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Conditions met" },
				new NTIWarningLetterStatus{ Id = 5, Name = "Escalate to Notice To Improve", Description="Conditions have not been met. Close NTI: Warning letter and begin NTI on case page using \"Add to case\".", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Escalated to Notice To Improve" }
			});
	}
}