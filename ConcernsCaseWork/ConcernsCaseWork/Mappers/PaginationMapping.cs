﻿using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Mappers
{
	public static class PaginationMapping
	{
		public static PaginationModel ToModel(Pagination paginationResponse)
		{
			if (paginationResponse == null)
			{
				return new PaginationModel();
			}

			var result = new PaginationModel()
			{
				Url = string.Empty,
				PageNumber = paginationResponse.Page,
				TotalPages = paginationResponse.TotalPages,
				Next = paginationResponse.HasNext ? paginationResponse.Page + 1 : null,
				Previous = paginationResponse.HasPrevious ? paginationResponse.Page - 1 : null,
				RecordCount = paginationResponse.RecordCount
			};

			return result;
		}
	}
}
