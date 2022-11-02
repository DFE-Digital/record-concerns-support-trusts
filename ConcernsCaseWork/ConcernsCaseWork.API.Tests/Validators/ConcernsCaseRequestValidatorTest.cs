using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.Validators;
using FizzWare.NBuilder;
using FluentValidation.TestHelper;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Validators
{
    public class ConcernsCaseRequestValidatorTest
    {
        [Fact]
        public void ShouldHaveError_WhenRatingUrnIs0()
        {
            var validator = new ConcernsCaseRequestValidator();
            
            var request = Builder<ConcernCaseRequest>.CreateNew()
                .With(c => c.RatingId = 0)
                .Build();
            
            var result = validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(c => c.RatingId)
                .WithErrorCode("GreaterThanOrEqualValidator");
        }
    }
}