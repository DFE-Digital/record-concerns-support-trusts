using ConcernsCaseWork.Models.CaseActions;
using ConcernsCasework.Service.Nti;

namespace ConcernsCaseWork.Mappers
{
	public static class NtiMappers
	{
		public static NtiModel ToServiceModel(NtiDto ntiDto)
		{
			return new NtiModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn
			};
		}
	}
}
