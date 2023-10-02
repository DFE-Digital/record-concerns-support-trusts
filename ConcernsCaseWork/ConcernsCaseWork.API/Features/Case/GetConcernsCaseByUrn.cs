using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IGetConcernsCaseByUrn
	{
		public ConcernsCaseResponse Execute(int urn);
	}

	public class GetConcernsCaseByUrn : IGetConcernsCaseByUrn
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;

		public GetConcernsCaseByUrn(IConcernsCaseGateway concernsCaseGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
		}

		public ConcernsCaseResponse Execute(int urn)
		{
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(urn);
			return concernsCase == null ? null : ConcernsCaseResponseFactory.Create(concernsCase);
		}
	}
}