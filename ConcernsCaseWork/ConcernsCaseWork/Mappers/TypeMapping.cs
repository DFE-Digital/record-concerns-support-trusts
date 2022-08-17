using ConcernsCaseWork.Models;
using ConcernsCasework.Service.Types;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class TypeMapping
	{
		public static TypeModel MapDtoToModel(IList<TypeDto> typesDto, long urn)
		{
			var selectedTypeDto = typesDto.FirstOrDefault(t => t.Urn.CompareTo(urn) == 0) ?? typesDto.First();
			
			return new TypeModel{ 
				Type = selectedTypeDto.Name ?? string.Empty,
				SubType = selectedTypeDto.Description ?? string.Empty
			};
		}
	}
}