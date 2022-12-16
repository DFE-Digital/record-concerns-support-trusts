using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests.DbGateways;

public class TestNoticeToImproveDbGateway : TestCaseDbGateway
{
	public NoticeToImprove GenerateTestNoticeToImproveInDb()
	{
		var parentCase = GenerateTestOpenCase();

		var noticeToImprove = _testDataFactory.BuildOpenNoticeToImprove(parentCase.Id, GetDefaultStatus().Id);

		using var context = CreateContext();
		context.NoticesToImprove.Add(noticeToImprove);
		context.SaveChanges();

		return GetNoticeToImproveCase(noticeToImprove.Id);
	}
	
	public NoticeToImprove UpdateNoticeToImprove(NoticeToImprove noticeToImprove)
	{
		using var context = CreateContext();
		context.NoticesToImprove.Update(noticeToImprove);
		context.SaveChanges();

		return GetNoticeToImproveCase(noticeToImprove.Id);
	}
	
	public NoticeToImprove GetNoticeToImproveCase(long noticeToImproveId)
	{
		using var context = CreateContext();
		return context
			.NoticesToImprove
			.Include(x => x.Status)
			.Single(x => x.Id == noticeToImproveId);
	}
	
	public NoticeToImproveStatus GetDefaultStatus()
	{
		using var context = CreateContext();
		return context.NoticeToImproveStatuses.First();
	}
		
	public NoticeToImproveStatus GetDifferentStatus(long? currentId)
	{
		using var context = CreateContext();
		return context.NoticeToImproveStatuses.First(x => x.Id != currentId);
	}
}