using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations.NtiUnderConsideration;

public class NTIUnderConsiderationReasonMappingConfiguration : IEntityTypeConfiguration<NTIUnderConsiderationReasonMapping>
{
	public void Configure(EntityTypeBuilder<NTIUnderConsiderationReasonMapping> builder)
	{
		builder.ToTable("NTIUnderConsiderationReasonMapping", "concerns");
		
		builder.HasKey(e => e.Id);
		
		builder
			.HasOne(n => n.NTIUnderConsideration)
			.WithMany(n => n.UnderConsiderationReasonsMapping)
			.HasForeignKey(n => n.NTIUnderConsiderationId);

		builder
			.HasOne(n => n.NTIUnderConsiderationReason)
			.WithMany(n => n.UnderConsiderationReasonsMapping)
			.HasForeignKey(n => n.NTIUnderConsiderationReasonId);
	}
}