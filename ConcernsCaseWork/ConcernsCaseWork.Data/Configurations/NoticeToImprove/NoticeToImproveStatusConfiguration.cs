using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NoticeToImprove;

public class NoticeToImproveStatusConfiguration : IEntityTypeConfiguration<NoticeToImproveStatus>
{
	public void Configure(EntityTypeBuilder<NoticeToImproveStatus> builder)
	{
		builder.ToTable("NoticeToImproveStatus", "concerns");

		builder.HasKey(e => e.Id);
		
		var createdAt = new DateTime(2022, 08, 17);

		builder.HasData(
			new NoticeToImproveStatus[]
			{
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.PreparingNTI, 
					Name = "Preparing NTI", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.IssuedNTI, 
					Name = "Issued NTI", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.ProgressOnTrack, 
					Name = "Progress on track", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.EvidenceOfNTINonCompliance, 
					Name = "Evidence of NTI non-compliance", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.SeriousNTIBreaches, 
					Name = "Serious NTI breaches - considering escalation to TWN", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.SubmissionToLiftNTIInProgress, 
					Name = "Submission to lift NTI in progress", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.SubmissionToCloseNTIInProgress, 
					Name = "Submission to close NTI in progress", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = false 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.Lifted, 
					Name = "Lifted", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = true 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.Closed, 
					Name = "Closed", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = true 
				},
				new NoticeToImproveStatus
				{ 
					Id = (int)API.Contracts.NoticeToImprove.NtiStatus.Cancelled, 
					Name = "Cancelled", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt, 
					IsClosingState = true 
				}
			});
	}
}