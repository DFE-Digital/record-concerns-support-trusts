namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class IfdDataModel
	{
		public string TrustType { get; }
		
		public GroupContactAddressModel GroupContactAddress { get; }
		
		public IfdDataModel(string trustType, GroupContactAddressModel groupContactAddress) => 
			(TrustType, GroupContactAddress) = (trustType, groupContactAddress);
	}
}