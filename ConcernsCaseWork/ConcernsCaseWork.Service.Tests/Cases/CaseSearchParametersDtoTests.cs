using ConcernsCaseWork.Service.Cases;

namespace ConcernsCaseWork.Service.Tests.Cases
{
    public class CaseSearchParametersDtoTests
    {
		[Test]
		public void Can_Set_And_Get_CaseOwners_And_TeamLeaders()
		{
			// Arrange
			var dto = new CaseSearchParametersDto();
            var owners = new List<string> { "owner1", "owner2" };
            var leaders = new List<string> { "leader1", "leader2" };

            // Act
            dto.CaseOwners = owners;
            dto.TeamLeaders = leaders;

			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(dto.CaseOwners, Is.EqualTo(owners));
				Assert.That(dto.TeamLeaders, Is.EqualTo(leaders));
			});
		}

		[Test]
		public void Properties_Are_Null_By_Default()
		{
			// Arrange
			var dto = new CaseSearchParametersDto();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(dto.CaseOwners, Is.Null);
				Assert.That(dto.TeamLeaders, Is.Null);
			});
		}
	}
}
