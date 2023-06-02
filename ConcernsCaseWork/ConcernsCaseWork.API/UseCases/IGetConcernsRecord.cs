using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
	public interface IGetConcernsRecord
	{
		ConcernsRecordResponse Execute(int id);
	}
}
