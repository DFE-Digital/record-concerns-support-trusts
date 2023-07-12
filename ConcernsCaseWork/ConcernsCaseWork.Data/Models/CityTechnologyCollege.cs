using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Models
{
	public class CityTechnologyCollege: IAuditable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string UKPRN { get; set; }
		public string CompaniesHouseNumber { get; set; }

		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string AddressLine3 { get; set; }
		public string Town { get; set; }
		public string County { get; set; }
		public string Postcode { get; set; }
	}
}
