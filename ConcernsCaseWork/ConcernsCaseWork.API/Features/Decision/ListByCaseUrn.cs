using AutoMapper;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Enums;

namespace ConcernsCaseWork.API.Features.Decision
{
	public class ListByCaseUrn
	{
		public class Query : IRequest<Result>
		{
			public int concernsCaseUrn { get; set; }
		}

		public class Result
		{
			public ICollection<ResultItem> Data { get; set; }

			public class ResultItem
			{
				public int ConcernsCaseUrn { get; set; }
				public int DecisionId { get; set; }
				public DateTimeOffset CreatedAt { get; set; }
				public DateTimeOffset UpdatedAt { get; set; }
				public string Title { get; set; }
				public DecisionStatus Status { get; set; }
				public DecisionOutcomeStatus? Outcome { get; set; }
				public DateTimeOffset? ClosedAt { get; set; }
			}

			public Result(ICollection<ResultItem> items)
			{
				this.Data = items;
			}
		}

		public class Handler : IRequestHandler<Query, Result>
		{
			private readonly ConcernsDbContext _context;
			private readonly MapperConfiguration _mapperConfiguration;

			public Handler(ConcernsDbContext context, MapperConfiguration mapperConfiguration)
			{
				_context = context;
				_mapperConfiguration = mapperConfiguration;
			}

			public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
			{
				var exp = _context.ConcernsCase.AsNoTracking()
									.Include(x => x.Decisions)
									.ThenInclude(x => x.DecisionTypes)
									.Include(x => x.Decisions)
									.ThenInclude(x => x.Outcome)
									.ThenInclude(x => x.BusinessAreasConsulted);
				
				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.concernsCaseUrn);

				var decisions = concernsCase.Decisions;

				var decisionResponseList = decisions.Select(decision => new Result.ResultItem()
				{
					ConcernsCaseUrn = concernsCase.Urn,
					DecisionId = decision.DecisionId,
					Status = (Contracts.Enums.DecisionStatus)decision.Status,
					Outcome = decision.Outcome?.Status,
					CreatedAt = decision.CreatedAt,
					UpdatedAt = decision.UpdatedAt,
					ClosedAt = decision.ClosedAt,
					Title = decision.GetTitle(),
				}).ToArray();

				var result = new Result(decisionResponseList);
				return result;
			}

		}
	}
}
