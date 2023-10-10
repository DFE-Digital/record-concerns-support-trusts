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

			RuleFor(x => x.Division).IsInEnum().WithMessage("Division must have value 1 or 2");

			RuleFor(x => x.Region).IsInEnum();
		}
	}
}