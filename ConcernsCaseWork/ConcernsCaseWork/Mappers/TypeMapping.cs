using ConcernsCaseWork.Models;
using Service.TRAMS.Types;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class TypeMapping
	{
		public static TypeModel MapDtoToModel(IList<TypeDto> typesDto, long id)
		{
			var selectedTypeDto = typesDto.FirstOrDefault(t => t.Id.CompareTo(id) == 0) ?? typesDto.First();
			
			return new TypeModel{ 
				Type = selectedTypeDto.Name ?? string.Empty,
				SubType = selectedTypeDto.Description ?? string.Empty
			};
		}
	}
}