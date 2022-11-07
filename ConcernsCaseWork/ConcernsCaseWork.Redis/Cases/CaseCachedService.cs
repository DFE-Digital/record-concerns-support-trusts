﻿using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Cases;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Cases
{
	public sealed class CaseCachedService : CachedService, ICaseCachedService
	{
		private readonly ILogger<CaseCachedService> _logger;
		private readonly ICaseService _caseService;
		private readonly ICaseSearchService _caseSearchService;

		public CaseCachedService(ICacheProvider cacheProvider, ICaseService caseService, ICaseSearchService caseSearchService, ILogger<CaseCachedService> logger)
			: base(cacheProvider)
		{
			_caseService = caseService;
			_caseSearchService = caseSearchService;
			_logger = logger;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusId)
		{
			_logger.LogInformation("CaseCachedService::GetCasesByCaseworkerAndStatus {Caseworker} - {StatusId}", caseworker, statusId);

			var userState = await GetData<UserState>(caseworker);
			if (userState != null)
			{
				var cachedCases = userState.CasesDetails.Values.Where(caseWrapper => caseWrapper.CaseDto.StatusId.CompareTo(statusId) == 0)
					.Select(caseWrapper => caseWrapper.CaseDto).ToList();

				if (cachedCases.Any())
				{
					return cachedCases;
				}
			}

			IList<CaseDto> casesDto = await _caseSearchService.GetCasesByCaseworkerAndStatus(new CaseCaseWorkerSearch(caseworker, statusId));
			if (!casesDto.Any())
			{
				return casesDto;
			}

			userState = await GetData<UserState>(caseworker);
			userState ??= new UserState(caseworker);

			Parallel.ForEach(casesDto, caseDto => userState.CasesDetails.TryAdd(caseDto.Urn, new CaseWrapper { CaseDto = caseDto }));

			await StoreData(caseworker, userState);

			return casesDto;
		}

		public async Task<CaseDto> GetCaseByUrn(string caseworker, long urn)
		{
			_logger.LogInformation("CaseCachedService::GetCaseByUrn {Caseworker} - {CaseUrn}", caseworker, urn);

			var userState = await GetData<UserState>(caseworker);
			if (userState != null && userState.CasesDetails.TryGetValue(urn, out var caseWrapper))
			{
				return caseWrapper.CaseDto;
			}

			var caseDto = await _caseService.GetCaseByUrn(urn);

			// Edge case fetching a case that doesn't belong to the logged casework
			// Trust overview shows all cases for a trust which some aren't from the logged caseworker
			if (!caseDto.CreatedBy.Equals(caseworker, StringComparison.OrdinalIgnoreCase)) return caseDto;

			userState ??= new UserState(caseworker);
			userState.CasesDetails.Add(urn, new CaseWrapper { CaseDto = caseDto });

			await StoreData(caseworker, userState);

			return caseDto;
		}

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			_logger.LogInformation("CaseCachedService::PostCase {Caseworker}", createCaseDto.CreatedBy);

			// Create case on Academies API
			var newCase = await _caseService.PostCase(createCaseDto);

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(newCase.CreatedBy);
			if (userState is null)
			{
				userState = new UserState(createCaseDto.CreatedBy) { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				userState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}

			await StoreData(newCase.CreatedBy, userState);

			return newCase;
		}

		public async Task PatchCaseByUrn(CaseDto caseDto)
		{
			_logger.LogInformation("CaseCachedService::PatchCaseByUrn {Caseworker} - {CaseUrn}", caseDto.CreatedBy, caseDto.Urn);

			// Patch case on Academies API
			var patchCaseDto = await _caseService.PatchCaseByUrn(caseDto);

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(patchCaseDto.CreatedBy);
			if (userState is null)
			{
				userState = new UserState(caseDto.CreatedBy) { CasesDetails = { { patchCaseDto.Urn, new CaseWrapper { CaseDto = patchCaseDto } } } };
			}
			else
			{
				if (userState.CasesDetails.TryGetValue(patchCaseDto.Urn, out var caseWrapper))
				{
					caseWrapper.CaseDto = patchCaseDto;
				}
				else
				{
					userState.CasesDetails.Add(patchCaseDto.Urn, new CaseWrapper { CaseDto = patchCaseDto });
				}
			}

			await StoreData(patchCaseDto.CreatedBy, userState);
		}
	}
}