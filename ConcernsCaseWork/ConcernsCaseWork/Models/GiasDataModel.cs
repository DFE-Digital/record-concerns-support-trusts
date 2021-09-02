namespace ConcernsCaseWork.Models
{
	public sealed class GiasDataModel
	{
		public string UkPrn { get; }

		public string GroupId { get; }

		public string GroupName { get; }

		public string CompaniesHouseNumber { get; }

		public GroupContactAddressModel GroupContactAddress { get; }

		public GiasDataModel(string ukprn, string groupId, string groupName, string companiesHouseNumber, GroupContactAddressModel groupContactAddress) =>
			(UkPrn, GroupId, GroupName, CompaniesHouseNumber, GroupContactAddress) = (ukprn, groupId, groupName, companiesHouseNumber, groupContactAddress);
	}
}