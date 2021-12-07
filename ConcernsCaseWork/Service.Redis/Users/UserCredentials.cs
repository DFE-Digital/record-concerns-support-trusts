namespace Service.Redis.Users
{
	/// <summary>
	/// NOTE: At the point of writing this class we
	/// don't know what AD will share for authentication
	/// UserName | Email
	/// </summary>
	public sealed class UserCredentials
	{
		public string UserName { get; }
		public string Email { get; }
		public string Password { get; }
		
		public UserCredentials(string userName, string email, string password) => (UserName, Email, Password) = (userName, email, password);
	}
}