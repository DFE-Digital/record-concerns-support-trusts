using AutoMapper;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.CityTechnicalCollege
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CityTechnologyCollege, List.Result.CityTechnologyCollege>();
			CreateMap<CityTechnologyCollege, GetByUKPRN.Result>();

		}
	}
}
