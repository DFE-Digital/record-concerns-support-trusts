using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.SRMA
{

	//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
	public class SRMACreatedNotification : INotification
	{
		public int Id { get; set; }
		public int CaseId { get; set; }
	}

	public class ConcernCreatedCaseUpdatedHandler : INotificationHandler<SRMACreatedNotification>
	{
		private readonly ConcernsDbContext _context;
		public ConcernCreatedCaseUpdatedHandler(ConcernsDbContext context)
		{
			_context = context;
		}

		public async Task Handle(SRMACreatedNotification notification, CancellationToken cancellationToken)
		{
			ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
			SRMACase srma = await _context.SRMACases.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
			cc.CaseLastUpdatedAt = srma.CreatedAt;
			await _context.SaveChangesAsync();
		}
	}

	public class SRMAUpdatedNotification : INotification
	{
		public int Id { get; set; }
		public int CaseId { get; set; }
	}

	public class SRMAUpdatedCaseUpdatedHandler : INotificationHandler<SRMAUpdatedNotification>
	{
		private readonly ConcernsDbContext _context;
		public SRMAUpdatedCaseUpdatedHandler(ConcernsDbContext context)
		{
			_context = context;
		}

		public async Task Handle(SRMAUpdatedNotification notification, CancellationToken cancellationToken)
		{
			ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
			SRMACase srma = await _context.SRMACases.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
			cc.CaseLastUpdatedAt = srma.UpdatedAt;
			await _context.SaveChangesAsync();
		}
	}
}
