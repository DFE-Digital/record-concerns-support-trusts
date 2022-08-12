using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.Nti;

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
