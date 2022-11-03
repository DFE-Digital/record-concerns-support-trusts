namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class StatusModel
	{
		public string Name { get; }
		
		public long Id { get; }
		
		public StatusModel(string name, long id) => 
			(Name, Id) = (name, id);
	}
}