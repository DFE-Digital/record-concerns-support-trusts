﻿using ConcernsCaseWork.Data;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Create
	{
		public class Command : IRequest<int>
		{
			public DateTime CreatedAt { get; set; }
			public DateTime UpdatedAt { get; set; }
			public DateTime ReviewAt { get; set; }
			public DateTime? ClosedAt { get; set; }

			[StringLength(300)]
			public string Name { get; set; }

			[StringLength(300)]
			public string Description { get; set; }

			[StringLength(300)]
			public string Reason { get; set; }
			public int CaseUrn { get; set; }
			public int TypeId { get; set; }
			public int RatingId { get; set; }
			public int StatusId { get; set; }
			public int MeansOfReferralId { get; set; }
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

			public async Task<int> Handle(Command request, CancellationToken cancellationToken)
			{
				ConcernsRecord cr = ConcernsRecord.Create(request.CaseUrn, request.TypeId, request.RatingId, request.MeansOfReferralId, request.StatusId);

				cr.ChangeNameDescriptionAndReason(request.Name, request.Description, request.Reason);
				cr.SetAuditInformation(request.CreatedAt, request.UpdatedAt, request.ReviewAt);

				_context.ConcernsRecord.Add(cr);
							
				await _context.SaveChangesAsync();

				var ConcernCreatedNotification = new ConcernCreatedNotification() { Id = cr.Id, CaseId = cr.CaseId, };
				await _mediator.Publish(ConcernCreatedNotification);

				return cr.Id;
			}
		}

		//ToDo: BB 25/07/2023 Once we have more events decide on specific folder structure to organise events and handles
		public class ConcernCreatedNotification : INotification
		{
			public int Id { get; set; }
			public int CaseId { get; set; }
		}

		public class ConcernCreatedCaseUpdatedHandler : INotificationHandler<ConcernCreatedNotification>
		{
			private readonly ConcernsDbContext _context;
			public ConcernCreatedCaseUpdatedHandler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task Handle(ConcernCreatedNotification notification, CancellationToken cancellationToken)
			{
				ConcernsCase cc = await _context.ConcernsCase.SingleOrDefaultAsync(f => f.Id == notification.CaseId, cancellationToken: cancellationToken);
				ConcernsRecord cr = await _context.ConcernsRecord.SingleOrDefaultAsync(f => f.Id == notification.Id, cancellationToken: cancellationToken);
				cc.CaseLastUpdatedAt = cr.CreatedAt;
				await _context.SaveChangesAsync();
			}
		}

	}
}
