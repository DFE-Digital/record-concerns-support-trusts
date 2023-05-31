using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
	public class DeleteConcernsRecord : IDeleteConcernsRecord
	{
		private readonly IConcernsRecordGateway _concernsRecordGateway;

		public DeleteConcernsRecord(IConcernsRecordGateway concernsRecordGateway)
		{
			_concernsRecordGateway = concernsRecordGateway;
		}

		public void Execute(int id)
		{
			_concernsRecordGateway.Delete(id);
		}
	}
}
