using ConcernsCaseWork.Extensions;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class GiasDataModel
	{
		public string UkPrn { get; }

		public string GroupId { get; }

		public string GroupName { get; }

		public string GroupNameTitle { get { return this.GroupName.ToTitle(); } }

		public string GroupTypeCode { get; }

		public string CompaniesHouseNumber { get; }

		public GroupContactAddressModel GroupContactAddress { get; }
		
		public GiasDataModel(string ukprn, string groupId, string groupName, string groupTypeCode, string companiesHouseNumber, GroupContactAddressModel groupContactAddress) =>
			(UkPrn, GroupId, GroupName, GroupTypeCode, CompaniesHouseNumber, GroupContactAddress) = 
			(ukprn, groupId, groupName.ToTitle(), groupTypeCode, companiesHouseNumber, groupContactAddress);
	}
}