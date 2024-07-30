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

			bool hasRelatedRecords = _context.ConcernsRecord.Any(f => f.CaseId == id) ||
						 _context.Decisions.Any(f => f.ConcernsCaseId == id) ||
						 _context.NoticesToImprove.Any(f => f.CaseUrn == id) ||
						 _context.NTIUnderConsiderations.Any(f => f.CaseUrn == id) ||
						 _context.NTIWarningLetters.Any(f => f.CaseUrn == id) ||
						 _context.FinancialPlanCases.Any(f => f.CaseUrn == id) ||
						 _context.SRMACases.Any(f => f.CaseUrn == id) ||
						 _context.TrustFinancialForecasts.Any(f => f.CaseUrn == id);

			if (hasRelatedRecords)
				return UseCaseResult.Failure(new UseCaseResultError($"Cannot deleted Case. Case has related concern(s) and case actions"));

			var concernsCase = _context.ConcernsCase.SingleOrDefault(f => f.Id == id);
			if (concernsCase == null)
				return UseCaseResult.Failure(new UseCaseResultError("Case not found."));

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
