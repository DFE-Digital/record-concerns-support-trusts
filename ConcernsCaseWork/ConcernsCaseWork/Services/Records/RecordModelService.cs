using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Types;
using Microsoft.Extensions.Logging;
using Service.Redis.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public sealed class RecordModelService : IRecordModelService
	{
		private readonly IRecordCachedService _recordCachedService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<RecordModelService> _logger;
		
		public RecordModelService(IRecordCachedService recordCachedService, 
			IRatingModelService ratingModelService,
			ITypeModelService typeModelService,
			ILogger<RecordModelService> logger)
		{
			_recordCachedService = recordCachedService;
			_ratingModelService = ratingModelService;
			_typeModelService = typeModelService;
			_logger = logger;
		}

		public async Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetRecordsModelByCaseUrn");

			var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseworker, caseUrn);
			var typesDto = await _typeModelService.GetTypes();
			var ratingsDto = await _ratingModelService.GetRatings();
			
			// Map the records dto to model
			var recordsModel = RecordMapping.MapDtoToModel(recordsDto, typesDto, ratingsDto);

			return recordsModel;
		}

		public async Task<RecordModel> GetRecordModelByUrn(string caseworker, long caseUrn, long urn)
		{
			_logger.LogInformation("RecordModelService::GetRecordModelByUrn");

			var records = await GetRecordsModelByCaseUrn(caseworker, caseUrn);

			if (!records.Any()) throw new Exception($"Case {caseUrn} does not contain any records");
			var recordModel = records.FirstOrDefault(r => r.Urn == urn) ?? records.First();

			return recordModel;
		}
	}
}
