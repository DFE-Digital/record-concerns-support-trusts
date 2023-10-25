using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NtiUnderConsideration;

public class NTIUnderConsiderationStatusConfiguration : IEntityTypeConfiguration<NTIUnderConsiderationStatus>
{
	public void Configure(EntityTypeBuilder<NTIUnderConsiderationStatus> builder)
	{
		builder.ToTable("NTIUnderConsiderationStatus", "concerns");
		
		builder.HasKey(e => e.Id);

		var createdAt = new DateTime(2022, 07, 12);

		builder.HasData(
			new NTIUnderConsiderationStatus[]
			{
				new NTIUnderConsiderationStatus
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationClosedStatus.NoFurtherAction, 
					Name = "No further action being taken", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				},
				new NTIUnderConsiderationStatus
				{ 
					Id = (int)API.Contracts.NtiUnderConsideration.NtiUnderConsiderationClosedStatus.ToBeEscalated, 
					Name = "To be escalated", 
					Description = "Warning letter or NTI can be set up using \"Add to case\".", 
					CreatedAt = createdAt, 
					UpdatedAt = createdAt 
				}
			});
	}
}