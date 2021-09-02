namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustDetailsModel
	{
		public GiasDataModel GiasData { get; }
		
		public TrustDetailsModel(GiasDataModel giasData) => (GiasData) = (giasData);
	}
}