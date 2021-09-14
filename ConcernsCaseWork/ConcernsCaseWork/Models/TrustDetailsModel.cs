using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustDetailsModel
	{
		public GiasDataModel GiasData { get; }
		
		public List<EstablishmentModel> Establishments { get; } 
		
		public TrustDetailsModel(GiasDataModel giasData, List<EstablishmentModel> establishments) => 
			(GiasData, Establishments) = (giasData, establishments);
	}
}