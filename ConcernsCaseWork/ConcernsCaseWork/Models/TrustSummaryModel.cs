using ConcernsCaseWork.Extensions;
using System.Collections.Generic;
using System.Text;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustSummaryModel
	{
		private readonly string _isNullOrEmpty = "-".PadRight(2);
		
		public string UkPrn { get; }
		
		public string Urn { get; }
		
		public string GroupName { get; }
		
		public string CompaniesHouseNumber { get; }
		
		public string TrustType { get; }
		
		public GroupContactAddressModel GroupContactAddress { get; }
		
		public List<EstablishmentSummaryModel> Establishments { get; }

		public string DisplayName
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append(string.IsNullOrEmpty(GroupName) ? _isNullOrEmpty : GroupName.ToTitle());
				sb.Append(",").Append(" ");
				sb.Append(string.IsNullOrEmpty(UkPrn) ? _isNullOrEmpty : UkPrn);
				sb.Append(",").Append(" ");
				sb.Append(string.IsNullOrEmpty(CompaniesHouseNumber) ? _isNullOrEmpty : CompaniesHouseNumber);
				sb.Append(" ");
				sb.Append(string.IsNullOrEmpty(GroupContactAddress?.Town) ? _isNullOrEmpty : $"({GroupContactAddress.Town})");
				
				return sb.ToString();
			}
		}

		public TrustSummaryModel(string ukprn, string urn, string groupName, 
			string companiesHouseNumber, string trustType, GroupContactAddressModel groupContactAddress, 
			List<EstablishmentSummaryModel> establishments) 
			=> (UkPrn, Urn, GroupName, CompaniesHouseNumber, TrustType, GroupContactAddress, Establishments) = 
				(ukprn, urn, groupName, companiesHouseNumber, trustType, groupContactAddress, establishments);
	}
}