namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustDetailsV4Dto
	{
		public string Name { get; set; }

		public string Ukprn { get; set; }

		public string CompaniesHouseNumber { get; set; }

		public GroupContactAddressV4Dto Address { get; set; }

		public GroupType Type { get; set; }

		public string ReferenceNumber { get; set; }
	}

	public class GroupType
	{
		public string Code { get; set; }

		public string Name { get; set; }
	}
}
