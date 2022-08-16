using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetConcernsCaseByUrn
    {
        public ConcernsCaseResponse Execute(int urn);
    }
}