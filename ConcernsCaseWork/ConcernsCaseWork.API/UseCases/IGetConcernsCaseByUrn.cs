using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCaseByUrn
    {
        public ConcernsCaseResponse Execute(int urn);
    }
}