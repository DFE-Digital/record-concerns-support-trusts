using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.FinancialPlan;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public sealed class FinancialPlanCachedService : CachedService, IFinancialPlanCachedService
	{
		private readonly ILogger<FinancialPlanCachedService> _logger;
		private readonly IFinancialPlanService _financialPlanService;

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
			if (caseWrapper.FinancialPlans?.Count > 0)
			{
				// if (!caseWrapper.FinancialPlans.Any()) return financialPlansDto;
				financialPlansDto = caseWrapper.FinancialPlans.Values.Select(r => r.FinancialPlanDto).ToList();
			}
			else
			{
				financialPlansDto = await _financialPlanService.GetFinancialPlansByCaseUrn(caseUrn);

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

			return financialPlansDto;
		}

		public async Task<FinancialPlanDto> GetFinancialPlansById(long caseUrn, long financialPlanId, string caseworker)
		{
			_logger.LogInformation("FinancialPlanCachedService::GetFinancialPlansById {Caseworker} - {CaseUrn} - {FinancialPlanID}", caseworker, caseUrn, financialPlanId);


			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);

			if (!userState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper))
			{
				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(caseUrn, financialPlanId);
				return financialPlanDto;
			}

			// Checking finance plan was cached before
			if (caseWrapper.FinancialPlans != null)
			{
				if (caseWrapper.FinancialPlans.TryGetValue(financialPlanId, out var financialPlanWrapper))
				{
					return financialPlanWrapper.FinancialPlanDto;
				}

				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(caseUrn, financialPlanId);
				caseWrapper.FinancialPlans.Add(financialPlanId, new FinancialPlanWrapper { FinancialPlanDto = financialPlanDto });

				return financialPlanDto;
			}
			else
			{
				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(caseUrn, financialPlanId);

				userState = await GetData<UserState>(caseworker);

				if (userState.CasesDetails.TryGetValue(caseUrn, out caseWrapper))
				{
					caseWrapper.FinancialPlans ??= new Dictionary<long, FinancialPlanWrapper>();

					caseWrapper.FinancialPlans.Add(financialPlanId, new FinancialPlanWrapper { FinancialPlanDto = financialPlanDto });

					await StoreData(caseworker, userState);
				}


				return financialPlanDto;
			}

		}

		public async Task PatchFinancialPlanById(FinancialPlanDto financialPlanDto, string caseworker)
		{
			_logger.LogInformation("FinancialPlanCachedService::PatchFinancialPlanById {Caseworker} - {CaseUrn}", caseworker, financialPlanDto.CaseUrn);

			// Patch financial plan on Academies API
			var patchFinancialPlanDto = await _financialPlanService.PatchFinancialPlanById(financialPlanDto);

			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return;

			// If case urn doesn't exist on the cache return
			if (!userState.CasesDetails.TryGetValue(patchFinancialPlanDto.CaseUrn, out var caseWrapper)) return;

			if (caseWrapper.FinancialPlans.TryGetValue(patchFinancialPlanDto.Id, out var financialPlanWrapper))
			{
				financialPlanWrapper.FinancialPlanDto = patchFinancialPlanDto;
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
			var createdFinancialPlanDto = await _financialPlanService.PostFinancialPlanByCaseUrn(createFinancialPlanDto);

			var userState = await GetData<UserState>(caseworker);
			if (userState is null) return createdFinancialPlanDto;

			// If case urn doesn't exist on the cache return new created financial plan
			if (!userState.CasesDetails.TryGetValue(createdFinancialPlanDto.CaseUrn, out var caseWrapper))
			{
				return createdFinancialPlanDto;
			}

			// Add new financial plan to the records cache
			caseWrapper.FinancialPlans ??= new ConcurrentDictionary<long, FinancialPlanWrapper>();
			caseWrapper.FinancialPlans.Add(createdFinancialPlanDto.Id, new FinancialPlanWrapper { FinancialPlanDto = createdFinancialPlanDto });

			await StoreData(caseworker, userState);

			return createdFinancialPlanDto;
		}
	}
}
