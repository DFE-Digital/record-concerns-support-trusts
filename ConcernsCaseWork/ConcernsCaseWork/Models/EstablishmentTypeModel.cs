namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class EstablishmentTypeModel
	{
		public string Name { get; }
		
		public string Code { get; }
		
		public EstablishmentTypeModel(string name, string code) => 
			(Name, Code) = 
			(name, code);
	}
}