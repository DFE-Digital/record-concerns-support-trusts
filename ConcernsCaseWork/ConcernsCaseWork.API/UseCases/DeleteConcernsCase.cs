using ConcernsCaseWork.Data;

namespace ConcernsCaseWork.API.UseCases
{
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

			bool HasConcernsRecord = _context.ConcernsRecord.Where(f => f.CaseId == id).Any();
			bool HasDecisions = _context.Decisions.Where(f => f.ConcernsCaseId == id).Any();
			bool HasNoticeToImprove = _context.NoticesToImprove.Where(f => f.CaseUrn == id).Any();
			bool HasNTIUnderconsideration = _context.NTIUnderConsiderations.Where(f => f.CaseUrn == id).Any();
			bool HasNTIWarningLetters = _context.NTIWarningLetters.Where(f => f.CaseUrn == id).Any();
			bool HasFinancialPlans = _context.FinancialPlanCases.Where(f => f.CaseUrn == id).Any();
			bool HasSRMAs = _context.SRMACases.Where(f => f.CaseUrn == id).Any();
			bool HasTFF = _context.TrustFinancialForecasts.Where(f => f.CaseUrn == id).Any();

			bool HasNoRelatedCaseActions = new[] { HasDecisions, HasNoticeToImprove, HasNTIUnderconsideration, HasNTIWarningLetters, HasFinancialPlans, HasSRMAs, HasTFF }.All(x => x == false);


			if (HasConcernsRecord && !HasNoRelatedCaseActions)
			{
				return UseCaseResult.Failure(new UseCaseResultError($"Cannot deleted Case. Case has related concern(s) and case actions"));
			}

			if (HasConcernsRecord)
			{
				return UseCaseResult.Failure(new UseCaseResultError($"Cannot deleted Case. Case has related concern(s)"));
			}

			if (!HasNoRelatedCaseActions)
			{
				return UseCaseResult.Failure(new UseCaseResultError($"Cannot deleted Case. Case has related case actions"));
			}
			var cc = this._context.ConcernsCase.SingleOrDefault(f => f.Id == id);
			cc.DeletedAt = System.DateTime.Now;
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


		public static UseCaseResult Success() => new (true, UseCaseResultError.Empty);
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
