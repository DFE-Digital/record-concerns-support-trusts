using System.Text;

namespace ConcernsCaseWork.Models
{
	public sealed class GroupContactAddressModel
	{
		private readonly string _isNullOrEmpty = "-".PadRight(2);
		
		public string Street { get; }
			
		public string Locality { get; }
			
		public string AdditionalLine { get; }
			
		public string Town { get; }
			
		public string County { get; }
			
		public string Postcode { get; }
			
		public string DisplayAddress
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append(string.IsNullOrEmpty(Street) ? _isNullOrEmpty : Street);
				sb.Append(",").Append(" ");
				sb.Append(string.IsNullOrEmpty(AdditionalLine) ? _isNullOrEmpty : AdditionalLine);
				sb.Append(",").Append(" ");
				sb.Append(string.IsNullOrEmpty(Locality) ? _isNullOrEmpty : Locality);
				sb.Append(",").Append(" ");
				sb.Append(string.IsNullOrEmpty(Postcode) ? _isNullOrEmpty : Postcode);
				
				return sb.ToString();
			}
		}
		
		public GroupContactAddressModel(string street, string locality, string additionalLine, string town, string county, string postcode) => 
			(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
	}
}