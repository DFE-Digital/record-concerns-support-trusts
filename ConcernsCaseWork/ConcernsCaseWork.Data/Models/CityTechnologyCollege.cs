using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Models
{
	public class CityTechnologyCollege: IAuditable
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string UKPRN { get; private set; }
		public string CompaniesHouseNumber { get; private set; }

		public string AddressLine1 { get; private set; }
		public string AddressLine2 { get; private set; }
		public string AddressLine3 { get; private set; }
		public string Town { get; private set; }
		public string County { get; private set; }
		public string Postcode { get; private set; }

		protected CityTechnologyCollege()
		{
			
		}
		private	CityTechnologyCollege(string name, string ukprn, string companiesHoueNumber)
		{
			this.Name = name;
			this.UKPRN = ukprn;
			this.CompaniesHouseNumber = companiesHoueNumber;
		}

		public static CityTechnologyCollege Create(string name, string ukprn, string companiesHoueNumber)
		{
			return new CityTechnologyCollege(name, ukprn, companiesHoueNumber);
		}

		public void ChangeAddress(string addressLine1, string addressLine2, string addressLine3, string town, string county, string postcode)
		{
			this.AddressLine1 = addressLine1;
			this.AddressLine2 = addressLine2;
			this.AddressLine3 = addressLine3;
			this.Town = town;
			this.County = county;
			this.Postcode = postcode;
		}
	}
}
