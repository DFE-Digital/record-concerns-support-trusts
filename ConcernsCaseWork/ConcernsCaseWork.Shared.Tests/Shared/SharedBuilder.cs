using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;
using System.Text;

namespace ConcernsCaseWork.Shared.Tests.Shared
{
	public static class SharedBuilder
	{
		public static string BuildDisplayName(TrustSearchDto trustSearchDto)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(trustSearchDto.GroupName) ? "-".PadRight(2) : trustSearchDto.GroupName.ToTitle());
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchDto.UkPrn) ? "-".PadRight(2) : trustSearchDto.UkPrn);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchDto.CompaniesHouseNumber) ? "-".PadRight(2) : trustSearchDto.CompaniesHouseNumber);
			sb.Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchDto.GroupContactAddress?.Town) ? "-".PadRight(2) : $"({trustSearchDto.GroupContactAddress.Town})");	
			
			return sb.ToString();
		}
		
		public static string BuildDisplayName(TrustSearchModel trustSearchModel)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(trustSearchModel.GroupName) ? "-".PadRight(2) : trustSearchModel.GroupName.ToTitle());
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchModel.UkPrn) ? "-".PadRight(2) : trustSearchModel.UkPrn);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchModel.CompaniesHouseNumber) ? "-".PadRight(2) : trustSearchModel.CompaniesHouseNumber);
			sb.Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSearchModel.GroupContactAddress?.Town) ? "-".PadRight(2) : $"({trustSearchModel.GroupContactAddress.Town})");	
			
			return sb.ToString();
		}
		
		public static string BuildDisplayAddress(GroupContactAddressDto groupContactAddressDto)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Street) ? "-".PadRight(2) : groupContactAddressDto.Street);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Locality) ? "-".PadRight(2) : groupContactAddressDto.Locality);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Town) ? "-".PadRight(2) : groupContactAddressDto.Town);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressDto.Postcode) ? "-".PadRight(2) : groupContactAddressDto.Postcode);
				
			return sb.ToString();
		}
		
		public static string BuildDisplayAddress(GroupContactAddressModel groupContactAddressModel)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(groupContactAddressModel.Street) ? "-".PadRight(2) : groupContactAddressModel.Street);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressModel.Locality) ? "-".PadRight(2) : groupContactAddressModel.Locality);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressModel.Town) ? "-".PadRight(2) : groupContactAddressModel.Town);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(groupContactAddressModel.Postcode) ? "-".PadRight(2) : groupContactAddressModel.Postcode);
				
			return sb.ToString();
		}
	}
}