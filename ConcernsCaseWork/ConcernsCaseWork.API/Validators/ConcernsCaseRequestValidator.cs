using ConcernsCaseWork.API.RequestModels;
using FluentValidation;

namespace ConcernsCaseWork.API.Validators
{
    public class ConcernsCaseRequestValidator : AbstractValidator<ConcernCaseRequest>
    {
        public ConcernsCaseRequestValidator()
        {
            RuleFor(x => x.RatingId).GreaterThanOrEqualTo(1)
                .WithMessage("Ratings Urn can not be 0");

            RuleFor(x => x.DivisionFK).GreaterThanOrEqualTo(1)
				.WithMessage("DivisionFK can not be 0");
		}
    }
}