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

namespace ConcernsCaseWork.API.Features.Decision
{
	public class GetByID
	{
		public class Query : IRequest<Result>
		{
			[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
			public int ConcernsCaseUrn { get; set; }

			[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
			public int DecisionId { get; set; }
		}

		public class Result
		{
			public int ConcernsCaseUrn { get; set; }
			public int DecisionId { get; set; }
			public DecisionTypeQuestion[] DecisionTypes { get; set; }
			public decimal TotalAmountRequested { get; set; }
			public string SupportingNotes { get; set; }
			public DateTimeOffset ReceivedRequestDate { get; set; }
			public string SubmissionDocumentLink { get; set; }
			public bool? SubmissionRequired { get; set; }
			public bool? RetrospectiveApproval { get; set; }
			public string CrmCaseNumber { get; set; }
			public DateTimeOffset CreatedAt { get; set; }
			public DateTimeOffset UpdatedAt { get; set; }
			public DecisionStatus DecisionStatus { get; set; }
			public DateTimeOffset? ClosedAt { get; set; }
			public string Title { get; set; }
			public DecisionOutcome? Outcome { get; set; }
			public bool IsEditable { get; set; }

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

				var concernsCase = exp.FirstOrDefault(c => c.Urn == request.ConcernsCaseUrn);
				var decision = concernsCase?.Decisions.FirstOrDefault(x => x.DecisionId == request.DecisionId);

				Result result = new Result()
				{
					ConcernsCaseUrn = concernsCase.Id,
					DecisionId = decision.DecisionId,
					DecisionTypes = decision.DecisionTypes.Select(x => {
						return new DecisionTypeQuestion()
						{
							Id = (Contracts.Enums.DecisionType)x.DecisionTypeId,
							DecisionDrawdownFacilityAgreedId = (Contracts.Enums.DecisionDrawdownFacilityAgreed?)x.DecisionDrawdownFacilityAgreedId,
							DecisionFrameworkCategoryId = (Contracts.Enums.DecisionFrameworkCategory?)x.DecisionFrameworkCategoryId
						};
					}).ToArray(),
					TotalAmountRequested = decision.TotalAmountRequested,
					SupportingNotes = decision.SupportingNotes,
					ReceivedRequestDate = decision.ReceivedRequestDate,
					SubmissionDocumentLink = decision.SubmissionDocumentLink,
					SubmissionRequired = decision.SubmissionRequired,
					RetrospectiveApproval = decision.RetrospectiveApproval,
					CrmCaseNumber = decision.CrmCaseNumber,
					CreatedAt = decision.CreatedAt,
					UpdatedAt = decision.UpdatedAt,
					ClosedAt = decision.ClosedAt,
					DecisionStatus = (Contracts.Enums.DecisionStatus)decision.Status,
					Title = decision.GetTitle(),
					Outcome = CreateDecisionOutcome(decision.Outcome),
					IsEditable = decision.ClosedAt == null
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
