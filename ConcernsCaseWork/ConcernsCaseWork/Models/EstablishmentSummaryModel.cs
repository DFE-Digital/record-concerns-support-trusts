namespace ConcernsCaseWork.Models
{
	public sealed class EstablishmentSummaryModel
	{
		public string Urn { get; }
		
		public string Name { get; }
		
		public string UkPrn { get; }
		
		public EstablishmentSummaryModel(string urn, string name, string ukprn) => 
			(Urn, Name, UkPrn) = (urn, name, ukprn);
	}
}