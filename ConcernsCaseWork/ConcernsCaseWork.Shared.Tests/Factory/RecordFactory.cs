using AutoFixture;
using ConcernsCaseWork.Models;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<RecordDto> BuildListRecordDto()
		{
			var currentDate = DateTimeOffset.Now;
			return new List<RecordDto>
			{
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 1, 1, 1,
					true, 1, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 2, 2, 2,
					true, 2, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 3, 3, 3,
					true, 3, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 4, 4, 2,
					true, 4, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 5, 5, 1,
					true, 5, 1)
			};
		}

		public static List<RecordDto> BuildListRecordDtoByCaseUrn(long caseUrn = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new List<RecordDto>
			{
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 1, 1,
					true, 1, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 2, 2,
					true, 2, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 3, 3,
					true, 3, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 4, 2,
					true, 4, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 5, 1,
					true, 5, 1)
			};
		}
		
		public static RecordDto BuildRecordDto(long caseUrn = 1, long typeUrn = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new RecordDto(currentDate, currentDate, currentDate, currentDate,
				Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, typeUrn, 1,
				true, 1, 1);
		}
		
		public static CreateRecordDto BuildCreateRecordDto(long caseUrn = 1, long typeUrn = 1, long ratingUrn = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new CreateRecordDto(currentDate, currentDate, currentDate, currentDate,
				Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, typeUrn, ratingUrn,
				true, 1);
		}
		
		public static RecordModel BuildRecordModel()
		{
			var currentDate = DateTimeOffset.Now;
			return new RecordModel(
				currentDate, 
				currentDate, 
				currentDate, 
				currentDate, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<long>(), 
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<bool>(),
				Fixture.Create<long>(),
				Fixture.Create<long>()
				);
		}
		
		public static List<RecordModel> BuildListRecordModel()
		{
			return new List<RecordModel> { BuildRecordModel() };
		}
	}
}