using ConcernsCaseWork.API.Contracts.Concerns;

namespace ConcernsCaseWork.API.Features.ConcernsType
{
	public class ConcernsTypeResponseFactory
	{
		public static ConcernsTypeResponse Create(Data.Models.ConcernsType concernsType)
		{
			return new ConcernsTypeResponse
			{
				Name = concernsType.Name,
				Description = concernsType.Description,
				CreatedAt = concernsType.CreatedAt,
				UpdatedAt = concernsType.UpdatedAt,
				Id = concernsType.Id
			};
		}
	}
}