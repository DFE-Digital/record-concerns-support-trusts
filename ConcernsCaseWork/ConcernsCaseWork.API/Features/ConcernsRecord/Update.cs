using MediatR;
using ConcernsCaseWork.Data;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.Data.Models;
	using Microsoft.EntityFrameworkCore;

	public class Update
	{
		public class ConcernModel
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
			public int? MeansOfReferralId { get; set; }
		}
		public class Command : IRequest<int>
		{
			public int Id { get; }

			public ConcernModel Model { get; set; }

			public Command(int id, ConcernModel model)
			{
				this.Id = id;
				this.Model = model;
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

			public async Task<int> Handle(Command request, CancellationToken cancellationToken)
			{
				ConcernsRecord cr = await _context.ConcernsRecord.SingleOrDefaultAsync(f => f.Id == request.Id);

				cr.CreatedAt = request.Model.CreatedAt;
				cr.UpdatedAt = request.Model.UpdatedAt;
				cr.ReviewAt = request.Model.ReviewAt;
				cr.ClosedAt = request.Model.ClosedAt;
				//Taken Previous Logic from Factory (ConcernsCaseWork.API.Factories ConcernsCaseFactory)
				cr.Name = request.Model.Name ?? cr.Name;
				cr.Description = request.Model.Description ?? cr.Name;
				cr.Reason = request.Model.Reason ?? cr.Name;

				cr.StatusId = request.Model.StatusId;
				cr.RatingId = request.Model.RatingId;
				cr.TypeId = request.Model.TypeId;
				if (request.Model.MeansOfReferralId.HasValue && request.Model.MeansOfReferralId > 0)
				{
					cr.MeansOfReferralId = request.Model.MeansOfReferralId;
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
