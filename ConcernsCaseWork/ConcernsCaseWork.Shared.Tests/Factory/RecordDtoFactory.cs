﻿using Service.TRAMS.Records;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordDtoFactory
	{
		public static List<RecordDto> BuildListRecordDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new List<RecordDto>
			{
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					"record-name", "record-description", "record-reason", 1, 1, 1,
					true, 1, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					"record-name", "record-description", "record-reason", 2, 2, 2,
					true, 2, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					"record-name", "record-description", "record-reason", 3, 3, 3,
					true, 3, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					"record-name", "record-description", "record-reason", 4, 4, 2,
					true, 4, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					"record-name", "record-description", "record-reason", 5, 5, 1,
					true, 5, 1)
			};
		}
	}
}