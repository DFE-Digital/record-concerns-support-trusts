using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConcernsCaseWork.Data.Configurations;

public class TrustFinancialForecastConfiguration : IEntityTypeConfiguration<TrustFinancialForecast>
{
	public void Configure(EntityTypeBuilder<TrustFinancialForecast> builder)
	{
		builder.ToTable("TrustFinancialForecast", "concerns");
		
		builder.HasKey(e => e.Id)
				.HasName("PK__TrustFinancialForecast");

		builder.Property(e => e.ForecastingToolRanAt)
			.HasConversion<string>();
		
		builder.Property(e => e.WasTrustResponseSatisfactory)
			.HasConversion<string>();
		
		builder.Property(e => e.SRMAOfferedAfterTFF)
			.HasConversion<string>();
	}
}