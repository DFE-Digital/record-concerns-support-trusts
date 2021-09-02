namespace Service.Redis.Models
{
	public sealed class UserCredentials
	{
		public string Email { get; }
		public string Password { get; }
		
		public UserCredentials(string email, string password) => (Email, Password) = (email, password);
	}
}