using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsCaseworkTeamMemberConfiguration : IEntityTypeConfiguration<ConcernsCaseworkTeamMember>
{
	public void Configure(EntityTypeBuilder<ConcernsCaseworkTeamMember> builder)
	{
		builder.ToTable("ConcernsCaseworkTeamMember", "concerns");
		
		builder.HasKey(e => e.TeamMemberId);
	}
}