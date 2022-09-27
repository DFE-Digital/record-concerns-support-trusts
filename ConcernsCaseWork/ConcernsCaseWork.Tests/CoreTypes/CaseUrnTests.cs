using ConcernsCaseWork.CoreTypes;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.CoreTypes;

public class CaseUrnTests
{
	[Test]
	public void CanCreateCaseUrn()
	{
		var sut = new CaseUrn(1);
		Assert.AreEqual(1, sut.Value);
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
		Assert.AreEqual(urnValue, sut.Value);
	}

	[Test]
	public void Can_Implicitly_Convert_CaseUrn_To_Long()
	{
		var sut = new CaseUrn(5);
		long result = (long)sut;
		Assert.AreEqual(sut.Value, result);
	}
}