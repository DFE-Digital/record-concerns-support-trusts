using AutoFixture;
using AutoFixture.Kernel;
using System;

namespace ConcernsCaseWork.API.Tests.Utils
{
    internal class RandomDateGenerator : ISpecimenBuilder
    {
        private readonly ISpecimenBuilder _innerGenerator;

        internal RandomDateGenerator(DateTime min, DateTime max)
        {
            _innerGenerator = new RandomDateTimeSequenceGenerator(min, max);
        }

        public object Create(object request, ISpecimenContext context)
        {
            var result = _innerGenerator.Create(request, context);
            if (result is NoSpecimen)
                return result;

            return ((DateTime)result).Date;
        }
    }
}
