using ConcernsCaseWork.Models;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class StatusMapping
	{
		public static StatusModel MapDtoToModel(IList<StatusDto> statusesDto, long id)
		{
			var selectedStatusDto = statusesDto.FirstOrDefault(s => s.Id.CompareTo(id) == 0) ?? statusesDto.First();

			return new StatusModel(selectedStatusDto.Name, selectedStatusDto.Id);
		}
	}
}