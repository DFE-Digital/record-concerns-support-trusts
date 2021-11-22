using AutoMapper;
using ConcernsCaseWork.Models;
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
		private readonly ILogger<RecordModelService> _logger;
		private readonly IMapper _mapper;

		public RecordModelService(IRecordCachedService recordCachedService, IMapper mapper, ILogger<RecordModelService> logger)
		{
			_recordCachedService = recordCachedService;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetRecordsModelByCaseUrn");

			var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseworker, caseUrn);

			// Map the records dto to model
			var recordsModel = _mapper.Map<IList<RecordModel>>(recordsDto);

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
