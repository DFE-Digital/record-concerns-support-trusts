using AutoFixture;
using ConcernsCaseWork.Models;
using Service.Redis.Models;
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
					1, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 2, 2, 2,
					2, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 3, 3, 3,
					3, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 4, 4, 2,
					4, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 5, 5, 1,
					5, 1)
			};
		}

		public static List<RecordDto> BuildListRecordDtoByCaseUrn(long caseUrn = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new List<RecordDto>
			{
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 1, 1,
					1, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 2, 2,
					2, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 3, 3,
					3, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 4, 2,
					4, 1),
				new RecordDto(currentDate, currentDate, currentDate, currentDate,
					Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, 5, 1,
					5, 1)
			};
		}
		
		public static RecordDto BuildRecordDto(long caseUrn = 1, long typeId = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new RecordDto(currentDate, currentDate, currentDate, currentDate,
				Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, typeId, 1,
				1, 1);
		}
		
		public static CreateRecordDto BuildCreateRecordDto(long caseUrn = 1, long typeId = 1, long ratingId = 1, long meansOfReferralUrn = 1)
		{
			var currentDate = DateTimeOffset.Now;
			return new CreateRecordDto(currentDate, currentDate, currentDate, currentDate,
				Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), caseUrn, typeId, ratingId,
				1, meansOfReferralUrn);
		}
		
		public static RecordModel BuildRecordModel(long statusId = 1)
		{
			return new RecordModel(
				Fixture.Create<long>(), 
				Fixture.Create<long>(),
				Fixture.Create<TypeModel>(),
				Fixture.Create<long>(),
				Fixture.Create<RatingModel>(),
				Fixture.Create<long>(),
				statusId,
				Fixture.Create<StatusModel>(),
				Fixture.Create<MeansOfReferralModel>()
			);
		}
		
		public static List<RecordModel> BuildListRecordModel()
		{
			return new List<RecordModel> { BuildRecordModel() };
		}

		public static PatchRecordModel BuildPatchRecordModel()
		{
			return new PatchRecordModel
			{
				UpdatedAt = Fixture.Create<DateTimeOffset>(),
				Id = 1,
				CaseUrn = 1,
				RatingId = 1,
				CreatedBy = Fixture.Create<string>(),
				StatusId = 1
			};
		}

		public static CreateRecordModel BuildCreateRecordModel()
		{
			return new CreateRecordModel
			{
				CaseUrn = Fixture.Create<long>(),
				Type = Fixture.Create<string>(),
				RagRating = Fixture.Create<Tuple<int, IList<string>>>(),
				RatingName = Fixture.Create<string>(),
				RatingId = Fixture.Create<long>(),
				SubType = Fixture.Create<string>(),
				TypeId = Fixture.Create<long>(),
				RagRatingCss = Fixture.Create<IList<string>>(),
				MeansOfReferralId = Fixture.Create<long>()
			};
		}
		
		public static IList<CreateRecordModel> BuildListCreateRecordModel()
		{
			return new List<CreateRecordModel> { BuildCreateRecordModel() };
		}
	}
}