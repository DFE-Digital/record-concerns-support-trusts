using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Configurations
{
	public class CityTechnologyCollegeConfiguration : IEntityTypeConfiguration<CityTechnologyCollege>
	{
		public void Configure(EntityTypeBuilder<CityTechnologyCollege> builder)
		{
			builder.ToTable("CityTechnologyCollege", "concerns");

			builder.HasKey(e => e.Id);

			builder.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(250);
			builder.Property(e => e.UKPRN)
				.HasMaxLength(12);
			builder.Property(x => x.CompaniesHouseNumber)
				.HasMaxLength(8);
			builder.Property(e => e.AddressLine1)
				.HasMaxLength(250);
			builder.Property(e => e.AddressLine2)
				.HasMaxLength(250);
			builder.Property(e => e.AddressLine3)
				.HasMaxLength(250);
			builder.Property(e => e.Town)
				.HasMaxLength(250);
			builder.Property(e => e.County)
				.HasMaxLength(250);
			builder.Property(e => e.Postcode)
				.HasMaxLength(9);

		}
	}
}
