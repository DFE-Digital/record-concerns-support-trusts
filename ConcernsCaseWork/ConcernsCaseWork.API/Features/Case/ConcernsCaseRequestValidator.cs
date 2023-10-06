using ConcernsCaseWork.API.Contracts.Case;
using FluentValidation;

namespace ConcernsCaseWork.API.Features.Case
{
	public class ConcernsCaseRequestValidator : AbstractValidator<ConcernCaseRequest>
	{
		public ConcernsCaseRequestValidator()
		{
			RuleFor(x => x.RatingId).GreaterThanOrEqualTo(1)
				.WithMessage("Ratings Urn can not be 0");

			RuleFor(x => (int?)x.Division).GreaterThanOrEqualTo(1)
				.WithMessage("Division can not be 0");
		}
	}
}