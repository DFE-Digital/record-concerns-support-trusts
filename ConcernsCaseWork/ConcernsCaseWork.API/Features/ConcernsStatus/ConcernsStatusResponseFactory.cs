using ConcernsCaseWork.API.Contracts.Concerns;

namespace ConcernsCaseWork.API.Features.ConcernsStatus
{
	public class ConcernsStatusResponseFactory
	{
		public static ConcernsStatusResponse Create(Data.Models.ConcernsStatus concernsStatus)
		{
			return new ConcernsStatusResponse
			{
				Name = concernsStatus.Name,
				CreatedAt = concernsStatus.CreatedAt,
				UpdatedAt = concernsStatus.UpdatedAt,
				Id = concernsStatus.Id
			};
		}
	}
}