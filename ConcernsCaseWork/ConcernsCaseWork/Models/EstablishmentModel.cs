namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class EstablishmentModel
	{
		public string Urn { get; }
		
		public string LocalAuthorityCode { get; }
		
		public string LocalAuthorityName { get; }
		
		public string EstablishmentNumber { get; }
		
		public string EstablishmentName { get; }
		
		public EstablishmentModel(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName);
	}
}