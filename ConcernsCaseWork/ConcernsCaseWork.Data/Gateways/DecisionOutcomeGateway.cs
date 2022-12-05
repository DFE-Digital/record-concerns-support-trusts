using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Data.Gateways
{
	public class DecisionOutcomeGateway : IDecisionOutcomeGateway
	{
		private readonly ConcernsDbContext _concernsDbContext;
		private readonly ILogger<FinancialPlanGateway> _logger;

		public DecisionOutcomeGateway(ConcernsDbContext concernsDbContext, ILogger<FinancialPlanGateway> logger)
		{
			_concernsDbContext = concernsDbContext;
			_logger = logger;
		}

		public async Task<DecisionOutcome> CreateDecisionOutcome(DecisionOutcome request, CancellationToken cancellationToken = default)
		{
			try
			{
				var result = _concernsDbContext.DecisionOutcomes.Add(request);
				await _concernsDbContext.SaveChangesAsync(cancellationToken);
				return result.Entity;
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError($"Failed to create Decision Outcome for Decision {request.DecisionId}, {ex}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError("An application exception has occurred whilst creating Decision Outcome for Decision {Id}, {ex}", request.DecisionId, ex);
				throw;
			}
		}

		public async Task<DecisionOutcome> UpdateDecisionOutcome(DecisionOutcome request, CancellationToken cancellationToken = default)
		{
			try
			{
				var toUpdate = await _concernsDbContext.Decisions
					.Include(x => x.Outcome)
					.Select(x => x.Outcome)
					.FirstAsync(x => x.DecisionId == request.DecisionId);

				toUpdate.Status = request.Status;
				toUpdate.TotalAmount = request.TotalAmount;
				toUpdate.Authorizer = request.Authorizer;
				toUpdate.BusinessAreasConsulted = request.BusinessAreasConsulted;
				toUpdate.DecisionEffectiveFromDate = request.DecisionEffectiveFromDate;
				toUpdate.DecisionMadeDate = request.DecisionMadeDate;

				await _concernsDbContext.SaveChangesAsync(cancellationToken);

				return toUpdate;

			}
			catch (DbUpdateException ex)
			{
				_logger.LogError($"Failed to update Decision Outcome for Decision {request.DecisionId}, {ex}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError("An application exception has occurred whilst updating Decision Outcome for Decision {Id}, {ex}", request.DecisionId, ex);
				throw;
			}
		}
	}

	public interface IDecisionOutcomeGateway
	{
		Task<DecisionOutcome> CreateDecisionOutcome(DecisionOutcome request, CancellationToken cancellationToken = default);
		Task<DecisionOutcome> UpdateDecisionOutcome(DecisionOutcome request, CancellationToken cancellationToken = default);
	}
}
