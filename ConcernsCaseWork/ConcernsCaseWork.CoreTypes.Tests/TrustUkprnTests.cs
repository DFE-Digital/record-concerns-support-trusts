namespace ConcernsCaseWork.CoreTypes.Tests;

public class TrustUkprnTests
{
	[Test]
	public void CanCreateTrustUkprnTests()
	{
		var sut = new TrustUkprn(1);
		Assert.That((long)sut, Is.EqualTo(1));
	}

	[Test]
	public void Cannot_Create_Negative_TrustUkprn()
	{
		Assert.Throws<ArgumentException>(() => new TrustUkprn(-1));
	}

	[Test]
	public void Can_Explicitly_Convert_Long_To_TrustUkprn()
	{
		long urnValue = 5;
		var sut = (TrustUkprn)urnValue;
		Assert.That((long)sut, Is.EqualTo(urnValue));
	}

	[Test]
	public void Can_Implicitly_Convert_TrustUkprn_To_Long()
	{
		long result = new TrustUkprn(5);
		Assert.That(result, Is.EqualTo(5));
	}

	[Test]
	public void TrustUkprn_Must_Be_Between_10000000_And_19999999()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => new TrustUkprn(9999999));
		Assert.Throws<ArgumentOutOfRangeException>(() => new TrustUkprn(20000000));
	}
}