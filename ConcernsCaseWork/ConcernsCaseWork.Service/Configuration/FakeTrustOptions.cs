namespace ConcernsCaseWork.Service.Configuration
{
	public class FakeTrustOptions
	{
		public FakeTrustOptions()
		{
			Trusts = new List<Trust>();
		}

		public const string FakeTrust = "FakeTrusts";

		public List<Trust> Trusts { get; set; }
	}

	public class Trust
	{
		public string Name { get; set; }
		public string UkPrn { get; set; }
		public string CompaniesHouseNumber { get; set; }
	}
}
