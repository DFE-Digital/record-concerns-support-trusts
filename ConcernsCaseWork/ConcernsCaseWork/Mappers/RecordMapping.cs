using Service.TRAMS.Records;
using Service.TRAMS.Type;
using System;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static CreateRecordDto Map(TypeDto typeDto, long caseUrn, long ratingUrn, long statusUrn, bool isPrimary)
		{
			var currentDate = DateTimeOffset.Now;

			return new CreateRecordDto(currentDate, currentDate, currentDate, currentDate, typeDto.Name,
				typeDto.Description, string.Empty, caseUrn, typeDto.Urn, ratingUrn, isPrimary,
				statusUrn);
		}
	}
}