using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustSummaryModel
	{
		private const string IsNullOrEmpty = "-";
		
		public string UkPrn { get; }
		
		public string Urn { get; }
		
		public string GroupName { get; }
		
		public string CompaniesHouseNumber { get; }
		
		public List<EstablishmentSummaryModel> Establishments { get; }

		public string DisplayName
		{
			get
			{
				return (string.IsNullOrEmpty(GroupName) ? IsNullOrEmpty : GroupName) + "," + 
				       (string.IsNullOrEmpty(Urn) ? IsNullOrEmpty : Urn) + "," + 
				       (string.IsNullOrEmpty(CompaniesHouseNumber) ? IsNullOrEmpty : CompaniesHouseNumber);
			}
		}

		public TrustSummaryModel(string ukprn, string urn, string groupName, string companiesHouseNumber, List<EstablishmentSummaryModel> establishments) 
			=> (UkPrn, Urn, GroupName, CompaniesHouseNumber, Establishments) = (ukprn, urn, groupName, companiesHouseNumber, establishments);
	}
}