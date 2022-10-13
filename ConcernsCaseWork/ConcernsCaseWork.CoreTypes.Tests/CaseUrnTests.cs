namespace ConcernsCaseWork.CoreTypes.Tests;

public class CaseUrnTests
{
	[Test]
	public void CanCreateCaseUrn()
	{
		var sut = new CaseUrn(1);
		Assert.That((long)sut, Is.EqualTo(1));
	}

	[Test]
	public void Cannot_Create_Negative_CaseUrn()
	{
		Assert.Throws<ArgumentException>(() => new CaseUrn(-1));
	}

	[Test]
	public void Can_Explicitly_Convert_Long_To_CaseUrn()
	{
		long urnValue = 5;
		var sut = (CaseUrn)urnValue;
		Assert.That((long)sut, Is.EqualTo(urnValue));
	}

	[Test]
	public void Can_Implicitly_Convert_CaseUrn_To_Long()
	{
		long result = new CaseUrn(5);
		Assert.That(result, Is.EqualTo(5));
	}
}