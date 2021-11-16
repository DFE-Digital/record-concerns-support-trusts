namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class IfdDataModel
	{
		public string TrustType { get; }

		public string TrustContactPhoneNumber { get; }

		public GroupContactAddressModel GroupContactAddress { get; }
		
		public IfdDataModel(string trustType, string trustContactPhoneNumber, GroupContactAddressModel groupContactAddress) => 
			(TrustType, TrustContactPhoneNumber, GroupContactAddress) = (trustType, trustContactPhoneNumber, groupContactAddress);
	}
}