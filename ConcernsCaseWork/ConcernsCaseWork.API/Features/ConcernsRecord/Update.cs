using ConcernsCaseWork.Data;
using MediatR;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.API.Contracts.Case;
	using ConcernsCaseWork.API.Exceptions;
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Update
	{
		public class Command : IRequest<int>
		{
			public int Id { get; }

			public ConcernsRecordRequest Request { get; set; }

			public Command(int id, ConcernsRecordRequest request)
			{
				Id = id;
				Request = request;
			}
		}

		public class CommandHandler : IRequestHandler<Command, Int32>
		{
			private readonly ConcernsDbContext _context;
			private readonly IMediator _mediator;

			public CommandHandler(ConcernsDbContext context, IMediator mediator)
			{
				_context = context;
				_mediator = mediator;
			}

			public async Task<int> Handle(Command command, CancellationToken cancellationToken)
			{
				ConcernsRecord cr = await _context.ConcernsRecord.SingleOrDefaultAsync(r => r.Id == command.Id && r.CaseId == command.Request.CaseUrn);

				if (cr == null)
				{
					throw new NotFoundException($"Concerns case {command.Request.CaseUrn} record {command.Id}");
				}

				cr.CreatedAt = command.Request.CreatedAt;
				cr.UpdatedAt = command.Request.UpdatedAt;
				cr.ReviewAt = command.Request.ReviewAt;
				cr.ClosedAt = command.Request.ClosedAt;
				//Taken Previous Logic from Factory (ConcernsCaseWork.API.Factories ConcernsCaseFactory)
				cr.Name = command.Request.Name ?? cr.Name;
				cr.Description = command.Request.Description ?? cr.Name;
				cr.Reason = command.Request.Reason ?? cr.Name;

				cr.StatusId = command.Request.StatusId;
				cr.RatingId = command.Request.RatingId;
				cr.TypeId = command.Request.TypeId;
				if (command.Request.MeansOfReferralId.HasValue && command.Request.MeansOfReferralId > 0)
				{
					cr.MeansOfReferralId = command.Request.MeansOfReferralId;
				}

				await _context.SaveChangesAsync();

				var ConcernCreatedNotification = new ConcernUpdatedNotification() { Id = cr.Id, CaseId = cr.CaseId, };
				await _mediator.Publish(ConcernCreatedNotification);

				return cr.Id;
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class ConcernUpdatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class ConcernUpdatedCaseUpdatedHandler : INotificationHandler<ConcernUpdatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public ConcernUpdatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(ConcernUpdatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				ConcernsRecord cr = await _context.ConcernsRecord.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = cr.UpdatedAt;
				await _context.SaveChangesAsync();
			}
		}
	}
}
