using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.UseCases
{
	public class GetConcernsRecord : IGetConcernsRecord
	{
		private readonly IConcernsRecordGateway _concernsRecordGateway;

		public GetConcernsRecord(IConcernsRecordGateway concernsRecordGateway)
		{
			_concernsRecordGateway = concernsRecordGateway;
		}

		public ConcernsRecordResponse Execute(int id)
		{
			ConcernsRecordResponse result = null;
			var record = _concernsRecordGateway.GetConcernsRecordByUrn(id);

			if (record != null)
			{
				result = ConcernsRecordResponseFactory.Create(record);
			}

			return result;
		}
	}
}
