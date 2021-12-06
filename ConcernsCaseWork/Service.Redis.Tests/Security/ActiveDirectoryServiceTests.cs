using NUnit.Framework;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Security
{
	/// <summary>
	/// TODO after integration with Azure AD update test.
	/// </summary>
	[Parallelizable(ParallelScope.All)]
	public class ActiveDirectoryServiceTests
	{
		[Test]
		public async Task WhenGetUserAsyncFromActiveDirectory_ReturnUserClaims()
		{
			// arrange
			var activeDirectoryService = new ActiveDirectoryService();
			var userCredentials = new UserCredentials("test.test", "test@email.com", "password");
			
			// act
			var userClaims = await activeDirectoryService.GetUserAsync(userCredentials);

			// assert
			Assert.That(userClaims, Is.Not.Null);
			Assert.That(userClaims, Is.InstanceOf<Claims>());
			Assert.That(userClaims.Email, Is.EqualTo(userCredentials.Email));
			Assert.That(userClaims.Id, Is.EqualTo(userCredentials.Password));
		}
	}
}