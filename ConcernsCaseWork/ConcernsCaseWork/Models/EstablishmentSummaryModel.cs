namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class EstablishmentSummaryModel
	{
		public string Urn { get; }
		
		public string Name { get; }
		
		public string UkPrn { get; }
		
		public EstablishmentSummaryModel(string urn, string name, string ukprn) => 
			(Urn, Name, UkPrn) = (urn, name, ukprn);
	}
}