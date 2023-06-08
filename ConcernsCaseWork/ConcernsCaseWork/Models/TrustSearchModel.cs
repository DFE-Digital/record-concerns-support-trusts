using ConcernsCaseWork.Extensions;
using System.Collections.Generic;
using System.Text;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustSearchModel
	{
		private readonly string _isNullOrEmpty = "-".PadRight(2);

		public string UkPrn { get; set; }

		public string Urn { get; set; }

		public string GroupName { get; set; }

		public string CompaniesHouseNumber { get; set; }

		public string TrustType { get; set; }

		public GroupContactAddressModel GroupContactAddress { get; set; }

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

		public TrustSearchModel()
		{
		}

		public TrustSearchModel(string ukprn, string urn, string groupName,
			string companiesHouseNumber, string trustType, GroupContactAddressModel groupContactAddress)
			=> (UkPrn, Urn, GroupName, CompaniesHouseNumber, TrustType, GroupContactAddress) =
				(ukprn, urn, groupName, companiesHouseNumber, trustType, groupContactAddress);
	}
}