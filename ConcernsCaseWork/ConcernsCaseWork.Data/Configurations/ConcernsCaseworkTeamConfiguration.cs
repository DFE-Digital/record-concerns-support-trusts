using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class ConcernsCaseworkTeamConfiguration : IEntityTypeConfiguration<ConcernsCaseworkTeam>
{
	public void Configure(EntityTypeBuilder<ConcernsCaseworkTeam> builder)
	{
		builder.ToTable("ConcernsCaseworkTeam", "concerns");
		
		builder.HasKey(e => e.Id);

		builder.HasMany(n => n.TeamMembers)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);
	}
}