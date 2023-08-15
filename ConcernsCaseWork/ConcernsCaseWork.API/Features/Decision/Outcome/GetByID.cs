using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Features.Decision.Outcome
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
			public int CaseUrn { get; set; }

			[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
			public int DecisionId { get; set; }
		}

		public class Result
		{
			public DecisionOutcome? Outcome { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result>
		{
			private readonly ConcernsDbContext _context;

			public Handler(ConcernsDbContext context)
			{
				_context = context;
			}

			public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
			{
				var exp = _context.ConcernsCase
						.Include(x => x.Decisions)
						.ThenInclude(x => x.DecisionTypes)
						.Include(x => x.Decisions)
						.ThenInclude(x => x.Outcome)
						.ThenInclude(x => x.BusinessAreasConsulted);

				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.CaseUrn);
				var decision = concernsCase?.Decisions.FirstOrDefault(x => x.DecisionId == request.DecisionId);

				Result result = new Result()
				{
					Outcome = CreateDecisionOutcome(decision.Outcome),
				};

				return result;
			
			}

			private DecisionOutcome CreateDecisionOutcome(Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome? entity)
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
