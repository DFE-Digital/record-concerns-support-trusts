using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Models.CaseActions
{
	[Parallelizable(ParallelScope.All)]
	public class SRMAModelTests
	{
		[Test]
		public void WhenBuildSRMAModel_ReturnsValidLogic()
		{
			// arrange
			var date = DateTime.Now;
			var notes = "Test Data";
			var status = SRMAStatus.Unknown;
			var reason = SRMAReasonOffered.Unknown;

			var srmaModel = new SRMAModel(
				1,
				1,
				date,
				date, 
				date, 
				date, 
				date, 
				status, 
				notes, 
				reason,
				date
			);
			
			// assert
			Assert.That(srmaModel.Id, Is.EqualTo(1));
			Assert.That(srmaModel.CaseUrn, Is.EqualTo(1));
			Assert.That(srmaModel.DateOffered, Is.EqualTo(date));
			Assert.That(srmaModel.DateAccepted, Is.EqualTo(date));
			Assert.That(srmaModel.DateReportSentToTrust, Is.EqualTo(date));
			Assert.That(srmaModel.DateVisitStart, Is.EqualTo(date));
			Assert.That(srmaModel.DateVisitEnd, Is.EqualTo(date));
			Assert.That(srmaModel.Status, Is.EqualTo(status));
			Assert.That(srmaModel.Notes, Is.EqualTo(notes));
			Assert.That(srmaModel.Reason, Is.EqualTo(reason));
		}
	}
}