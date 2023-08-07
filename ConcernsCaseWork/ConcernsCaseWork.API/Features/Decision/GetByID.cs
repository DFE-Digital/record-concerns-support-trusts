using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.API.Features.Decision
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			public int Id { get; set; }
		}

		public class Result
		{
			private const int _maxUrlLength = 2048;
			private const int _maxCaseNumberLength = 20;

			public int Id { get; set; }
			public int ConcernsCaseUrn { get; set; }
			public DecisionTypeQuestion[] DecisionTypes { get; set; }

			public decimal TotalAmountRequested { get; set; }

			public string SupportingNotes { get; set; }

			public DateTimeOffset? ReceivedRequestDate { get; set; }

			public string SubmissionDocumentLink { get; set; }

			public bool? SubmissionRequired { get; set; }

			public bool? RetrospectiveApproval { get; set; }

			public string CrmCaseNumber { get; set; }

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

				var result = _context.Decisions
								.Include(x => x.DecisionTypes)
								.Include(x => x.Outcome)
								.ThenInclude(x => x.BusinessAreasConsulted).ProjectTo<Result>(_mapperConfiguration).SingleOrDefaultAsync(f => f.Id == request.Id);


				return null;
			}
		}
	}
}
