using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Mappers
{
	public static class NtiMappers
	{
		public static NtiDto ToDBModel(NtiModel ntiModel)
		{
			return new NtiDto
			{
				Id = ntiModel.Id,
				CaseUrn = ntiModel.CaseUrn,
				Reasons = EnumToReasons(ntiModel.NtiReasonForConsidering)
			};
		}

		public static NtiModel ToServiceModel(NtiDto ntiDto)
		{
			return new NtiModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
			};
		}

		private static ICollection<NtiReasonDto> EnumToReasons(NtiReasonForConsidering ntiReasonForConsidering)
		{
			throw new NotImplementedException();
		}
	}
}
