using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.Decision
{
	public class GetByID
	{
		public class Query : IRequest<GetDecisionResponse>
		{
			[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
			public int ConcernsCaseUrn { get; set; }

			[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
			public int DecisionId { get; set; }
		}

		public class Handler : IRequestHandler<Query, GetDecisionResponse>
		{
			private readonly ConcernsDbContext _context;

			public Handler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<GetDecisionResponse> Handle(Query request, CancellationToken cancellationToken)
			{
				var concernCase = await _context.ConcernsCase
					.Include(x => x.Decisions)
					.ThenInclude(x => x.DecisionTypes)
					.Include(x => x.Decisions)
					.ThenInclude(x => x.Outcome)
					.ThenInclude(x => x.BusinessAreasConsulted)
					.FirstOrDefaultAsync(o => o.Urn == request.ConcernsCaseUrn);

				if (concernCase == null)
				{
					throw new NotFoundException($"Concerns case with id {request.ConcernsCaseUrn}");
				}

				var decision = concernCase.Decisions.FirstOrDefault(d => d.DecisionId == request.DecisionId);

				if (decision == null)
				{
					throw new NotFoundException($"Decision with id {request.DecisionId}, Case {request.ConcernsCaseUrn}");
				}

				var result = new GetDecisionResponse()
				{
					ConcernsCaseUrn = concernCase.Id,
					DecisionId = decision.DecisionId,
					DecisionTypes = decision.DecisionTypes.Select(x => {
						return new DecisionTypeQuestion()
						{
							Id = (DecisionType)x.DecisionTypeId,
							DecisionDrawdownFacilityAgreedId = x.DecisionDrawdownFacilityAgreedId,
							DecisionFrameworkCategoryId = x.DecisionFrameworkCategoryId
						};
					}).ToArray(),
					TotalAmountRequested = decision.TotalAmountRequested,
					SupportingNotes = decision.SupportingNotes,
					ReceivedRequestDate = decision.ReceivedRequestDate,
					SubmissionDocumentLink = decision.SubmissionDocumentLink,
					SubmissionRequired = decision.SubmissionRequired,
					RetrospectiveApproval = decision.RetrospectiveApproval,
					CrmCaseNumber = decision.CrmCaseNumber,
					HasCrmCase = decision.HasCrmCase,
					CreatedAt = decision.CreatedAt,
					UpdatedAt = decision.UpdatedAt,
					ClosedAt = decision.ClosedAt,
					DecisionStatus = (DecisionStatus)decision.Status,
					Title = decision.GetTitle(),
					Outcome = CreateDecisionOutcome(decision.Outcome),
					IsEditable = decision.ClosedAt == null
				};

				return result;
			
			}

			private DecisionOutcome CreateDecisionOutcome(Data.Models.Decisions.Outcome.DecisionOutcome? entity)
			{
				if (entity == null)
				{
					return null;
				}

				var result = new DecisionOutcome()
				{
					DecisionOutcomeId = entity.DecisionOutcomeId,
					Status = entity.Status,
					Authorizer = entity.Authorizer,
					DecisionEffectiveFromDate = entity.DecisionEffectiveFromDate,
					DecisionMadeDate = entity.DecisionMadeDate,
					TotalAmount = entity.TotalAmount,
					BusinessAreasConsulted = entity.BusinessAreasConsulted.Select(b => b.DecisionOutcomeBusinessId).ToList()
				};

				return result;
			}
		}
	}
}
