using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CredentialModel
	{
		[Required]
		public string UserName { get; set; }
			
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
			
		public string ReturnUrl { get; set; }

		public bool Validate()
		{
			return string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password);
		}
	}
}