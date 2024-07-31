using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IDeleteConcernsCase
	{
		UseCaseResult Execute(int id);
	}

	public class DeleteConcernsCase : IDeleteConcernsCase
	{
		/// <summary>
		/// Injecting Datacontext here as more flexible and performant.
		/// </summary>
		private readonly ConcernsDbContext _context;

		public DeleteConcernsCase(ConcernsDbContext concernsDbContext)
		{
			_context = concernsDbContext;
		}

		public UseCaseResult Execute(int id)
		{
			var concernsCase = _context.ConcernsCase.SingleOrDefault(f => f.Id == id);
			if (concernsCase == null)
				return UseCaseResult.Failure(new UseCaseResultError("Case not found."));

			var hasRelatedRecords = _context.ConcernsCase
				.Where(cr => cr.Id == id)
				.Select(cr => new
				{
					HasRelatedRecords = _context.ConcernsRecord.Any(d => d.CaseId == id) ||
										_context.Decisions.Any(d => d.ConcernsCaseId == id) ||
										_context.NoticesToImprove.Any(nti => nti.CaseUrn == id) ||
										_context.NTIUnderConsiderations.Any(ntiuc => ntiuc.CaseUrn == id) ||
										_context.NTIWarningLetters.Any(ntiwl => ntiwl.CaseUrn == id) ||
										_context.FinancialPlanCases.Any(fp => fp.CaseUrn == id) ||
										_context.SRMACases.Any(srma => srma.CaseUrn == id) ||
										_context.TrustFinancialForecasts.Any(tff => tff.CaseUrn == id)
				})
				.FirstOrDefault();

			if (hasRelatedRecords != null && hasRelatedRecords.HasRelatedRecords)
				return UseCaseResult.Failure(new UseCaseResultError($"Cannot deleted Case. Case has related concern(s) or case actions"));

			concernsCase.DeletedAt = DateTime.Now;
			_context.SaveChanges();

			return UseCaseResult.Success();
		}
	}

	public class UseCaseResult
	{
		protected UseCaseResult(bool IsSuccess, UseCaseResultError error)
		{
			this.IsSuccess = IsSuccess;
			Error = error;
		}

		public bool IsSuccess { get; private set; }
		public bool IsFailure => !IsSuccess;
		public UseCaseResultError Error { get; set; }


		public static UseCaseResult Success() => new(true, UseCaseResultError.Empty);
		public static UseCaseResult Failure(UseCaseResultError error) => new(false, error);
	}

	public class UseCaseResultError
	{
		public string Message { get; }

		public UseCaseResultError(string message)
		{
			Message = message;
		}

		public static UseCaseResultError Empty => new(string.Empty);

	}
}
