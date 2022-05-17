using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public sealed class FinancialPlanCachedService : CachedService, IFinancialPlanCachedService
	{
		private readonly ILogger<FinancialPlanCachedService> _logger;
		private readonly IFinancialPlanService _financialPlanService;

		private readonly SemaphoreSlim _semaphoreFinancialPlan = new SemaphoreSlim(1, 1);

		public FinancialPlanCachedService(ICacheProvider cacheProvider, IFinancialPlanService financialPlanService, ILogger<FinancialPlanCachedService> logger)
		: base(cacheProvider)
		{
			_financialPlanService = financialPlanService;
			_logger = logger;
		}

		public async Task<IList<FinancialPlanDto>> GetFinancialPlansByCaseUrn(long caseUrn, string caseworker)
		{
			_logger.LogInformation("FinancialPlanCachedService::GetFinancialPlansByCaseUrn {Caseworker} - {CaseUrn}", caseworker, caseUrn);

			IList<FinancialPlanDto> financialPlansDto = new List<FinancialPlanDto>();

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return financialPlansDto;

			// If case urn doesn't exist on the cache may mean it belongs to another caseworker
			// Edge case fetching financialPlans that don't belong to the logged casework
			if (!userState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper))
			{
				financialPlansDto = await _financialPlanService.GetFinancialPlansByCaseUrn(caseUrn);

				return financialPlansDto;
			}

			// Checking finance plan was cached before
			if (caseWrapper.FinancialPlans != null)
			{
				if (!caseWrapper.FinancialPlans.Any()) return financialPlansDto;
				financialPlansDto = caseWrapper.FinancialPlans.Values.Select(r => r.FinancialPlanDto).ToList();
			}
			else
			{
				financialPlansDto = await _financialPlanService.GetFinancialPlansByCaseUrn(caseUrn);

				try
				{
					await _semaphoreFinancialPlan.WaitAsync();

					userState = await GetData<UserState>(caseworker);

					if (userState.CasesDetails.TryGetValue(caseUrn, out caseWrapper))
					{
						caseWrapper.FinancialPlans ??= new Dictionary<long, FinancialPlanWrapper>();

						if (financialPlansDto.Any())
						{
							caseWrapper.FinancialPlans = financialPlansDto.ToDictionary(financialPlanDto => financialPlanDto.Id, financialPlanDto => new FinancialPlanWrapper { FinancialPlanDto = financialPlanDto });
						}

						await StoreData(caseworker, userState);
					}
				}
				finally
				{
					_semaphoreFinancialPlan.Release();
				}
			}

			return financialPlansDto;
		}

		public async Task PatchFinancialPlanById(FinancialPlanDto financialPlanDto, string caseworker)
		{
			_logger.LogInformation("FinancialPlanCachedService::PatchFinancialPlanById {Caseworker} - {CaseUrn}", caseworker, financialPlanDto.CaseUrn);

			// Patch financial plan on Academies API
			//var patchFinancialPlanDto = await _financialPlanService.PatchFinancialPlanById(financialPlanDto);
			var patchFinancialPlanDto = financialPlanDto;

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return;

			// If case urn doesn't exist on the cache return
			if (!userState.CasesDetails.TryGetValue(patchFinancialPlanDto.CaseUrn, out var caseWrapper)) return;

			if (caseWrapper.FinancialPlans.TryGetValue(patchFinancialPlanDto.Id, out var recordWrapper))
			{
				recordWrapper.FinancialPlanDto = patchFinancialPlanDto;
			}
			else
			{
				caseWrapper.FinancialPlans.Add(patchFinancialPlanDto.Id, new FinancialPlanWrapper { FinancialPlanDto = patchFinancialPlanDto });
			}

			await StoreData(caseworker, userState);
		}

		public async Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanDto createFinancialPlanDto, string caseworker)
		{
			_logger.LogInformation("FinancialPlanCachedService::PostFinancialPlanByCaseUrn {Caseworker} - {CaseUrn}", caseworker, createFinancialPlanDto.CaseUrn);

			// Create financial plan on Academies API
			//var newFinancialPlanDto = await _financialPlanService.PostFinancialPlanByCaseUrn(createFinancialPlanDto);
			var newFinancialPlanDto = MapCreateDtoToDto(createFinancialPlanDto);

			try
			{
				await _semaphoreFinancialPlan.WaitAsync();

				var userState = await GetData<UserState>(caseworker);
				if (userState is null) return newFinancialPlanDto;

				// If case urn doesn't exist on the cache return new created financial plan
				if (!userState.CasesDetails.TryGetValue(newFinancialPlanDto.CaseUrn, out var caseWrapper))
				{
					return newFinancialPlanDto;
				}

				// Add new record to the records cache
				caseWrapper.FinancialPlans ??= new ConcurrentDictionary<long, FinancialPlanWrapper>();
				caseWrapper.FinancialPlans.Add(newFinancialPlanDto.Id, new FinancialPlanWrapper { FinancialPlanDto = newFinancialPlanDto });

				await StoreData(caseworker, userState);
			}
			finally
			{
				_semaphoreFinancialPlan.Release();
			}

			return newFinancialPlanDto;
		}


		// TODO - Remove once API has been implemented
		private FinancialPlanDto MapCreateDtoToDto(CreateFinancialPlanDto createFinancialPlanDto)
		{

			var random = new Random(DateTime.Now.Millisecond);

			var id = random.Next(10000);

			return new FinancialPlanDto(id,
				createFinancialPlanDto.CaseUrn,
				createFinancialPlanDto.CreatedAt,
				null,
				createFinancialPlanDto.CreatedBy,
				createFinancialPlanDto.StatusId,
				createFinancialPlanDto.DatePlanRequested,
				createFinancialPlanDto.DateViablePlanReceived,
				createFinancialPlanDto.Notes
				);
		}
	}
}
