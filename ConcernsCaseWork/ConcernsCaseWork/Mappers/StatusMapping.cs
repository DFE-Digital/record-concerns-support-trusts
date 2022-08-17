using ConcernsCaseWork.Models;
using ConcernsCasework.Service.Status;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class StatusMapping
	{
		public static StatusModel MapDtoToModel(IList<StatusDto> statusesDto, long urn)
		{
			var selectedStatusDto = statusesDto.FirstOrDefault(s => s.Urn.CompareTo(urn) == 0) ?? statusesDto.First();

			return new StatusModel(selectedStatusDto.Name, selectedStatusDto.Urn);
		}
	}
}