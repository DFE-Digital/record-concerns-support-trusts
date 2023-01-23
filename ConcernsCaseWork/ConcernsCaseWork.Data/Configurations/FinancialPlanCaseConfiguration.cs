using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class FinancialPlanCaseConfiguration : IEntityTypeConfiguration<FinancialPlanCase>
{
	public void Configure(EntityTypeBuilder<FinancialPlanCase> builder)
	{
		builder.ToTable("FinancialPlanCase", "concerns");
		
		builder.HasKey(e => e.Id);
	}
}
