using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestNtiWarningLetterDbGateway : TestCaseDbGateway
{
	public NTIWarningLetter GenerateTestNtiWarningLetter()
	{
		var parentCase = GenerateTestOpenCase();

		var ntiWarningLetter = _testDataFactory.BuildOpenNtiWarningLetter(parentCase.Id, GetDefaultNonClosedStatus().Id);

		using var context = CreateContext();
		context.NTIWarningLetters.Add(ntiWarningLetter);
		context.SaveChanges();

		return GetNtiWarningLetterCase(ntiWarningLetter.Id);
	}
	
	public NTIWarningLetter UpdateNtiWarningLetter(NTIWarningLetter ntiWarningLetter)
	{
		using var context = CreateContext();
		context.NTIWarningLetters.Update(ntiWarningLetter);
		context.SaveChanges();

		return GetNtiWarningLetterCase(ntiWarningLetter.Id);
	}
	
	public NTIWarningLetter GetNtiWarningLetterCase(long ntiWarningLetterId)
	{
		using var context = CreateContext();
		return context
			.NTIWarningLetters
			.Include(x => x.ClosedStatus)
			.Include(x => x.Status)
			.Single(x => x.Id == ntiWarningLetterId);
	}

	public NTIWarningLetterStatus GetDefaultNonClosedStatus()
	{
		using var context = CreateContext();
		return context.NTIWarningLetterStatuses.First(x => !x.IsClosingState);
	}
	
	public NTIWarningLetterStatus GetDifferentNonClosedStatus(long? currentId)
	{
		using var context = CreateContext();
		return context.NTIWarningLetterStatuses.First(x => !x.IsClosingState && x.Id != currentId);
	}
	
	public NTIWarningLetterStatus GetDefaultClosedStatus()
	{
		using var context = CreateContext();
		return context.NTIWarningLetterStatuses.First(x => x.IsClosingState);
	}
}