using ConcernsCaseWork.API.Contracts.Case;
using FluentValidation;

namespace ConcernsCaseWork.API.Features.Case
{
	public class ConcernsCaseRequestValidator : AbstractValidator<ConcernCaseRequest>
	{
		public ConcernsCaseRequestValidator()
		{
			RuleFor(x => x.RatingId).GreaterThanOrEqualTo(1)
				.WithMessage("'RatingId' must be a value within range");

			RuleFor(x => x.Division).NotEmpty().IsInEnum().WithMessage("'Division' must be a value within range");

			RuleFor(x => x).Must(HasTerritoryOrRegion).WithMessage("'Territory' or 'Region' must be provided");

			RuleFor(x => x.Region).IsInEnum().WithMessage("'Region' must be a value within range");
			RuleFor(x => x.Territory).IsInEnum().WithMessage("'Territory' must be a value within range");
			RuleFor(x => x.TrustUkprn).NotEmpty().WithMessage("'TrustUkprn' must not be empty");
		}

		public bool HasTerritoryOrRegion(ConcernCaseRequest request)
		{
			return request.Territory != null || request.Region != null;
		}
	}
}