using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
using System.Text;

namespace ConcernsCaseWork.Shared.Tests.Shared
{
	public static class SharedBuilder
	{
		public static string BuildDisplayName(TrustSummaryDto trustSummaryDto)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.GroupName) ? "-".PadRight(2) : trustSummaryDto.GroupName.ToTitle());
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.UkPrn) ? "-".PadRight(2) : trustSummaryDto.UkPrn);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.CompaniesHouseNumber) ? "-".PadRight(2) : trustSummaryDto.CompaniesHouseNumber);
			sb.Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryDto.GroupContactAddress?.Town) ? "-".PadRight(2) : $"({trustSummaryDto.GroupContactAddress.Town})");	
			
			return sb.ToString();
		}
		
		public static string BuildDisplayName(TrustSummaryModel trustSummaryModel)
		{
			var sb = new StringBuilder();
			sb.Append(string.IsNullOrEmpty(trustSummaryModel.GroupName) ? "-".PadRight(2) : trustSummaryModel.GroupName.ToTitle());
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryModel.UkPrn) ? "-".PadRight(2) : trustSummaryModel.UkPrn);
			sb.Append(",").Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryModel.CompaniesHouseNumber) ? "-".PadRight(2) : trustSummaryModel.CompaniesHouseNumber);
			sb.Append(" ");
			sb.Append(string.IsNullOrEmpty(trustSummaryModel.GroupContactAddress?.Town) ? "-".PadRight(2) : $"({trustSummaryModel.GroupContactAddress.Town})");	
			
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