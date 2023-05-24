using ConcernsCaseWork.Extensions;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class GiasDataModel
	{
		public string UkPrn { get; set; }

		public string GroupId { get; set; }

		public string GroupName { get; set; }

		public string GroupNameTitle { get { return GroupName.ToTitle(); } }

		public string GroupTypeCode { get; set; }
		
		public string GroupType { get; set; }

		public string CompaniesHouseNumber { get; set; }

		public string CompaniesHouseWebsite { get { return $"https://find-and-update.company-information.service.gov.uk/company/{CompaniesHouseNumber}"; } }

		public GroupContactAddressModel GroupContactAddress { get; set; }

		public GiasDataModel()
		{ 
		}
		
		public GiasDataModel(string ukprn, string groupId, string groupName, string groupTypeCode, string companiesHouseNumber, GroupContactAddressModel groupContactAddress, string groupType) =>
			(UkPrn, GroupId, GroupName, GroupTypeCode, CompaniesHouseNumber, GroupContactAddress, GroupType) = 
			(ukprn, groupId, groupName.ToTitle(), groupTypeCode, companiesHouseNumber, groupContactAddress, groupType);
	}
}