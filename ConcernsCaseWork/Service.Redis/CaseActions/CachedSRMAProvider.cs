using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.CaseActions
{
	public class CachedSRMAProvider : CachedService
	{
		private readonly ICacheProvider _cacheProvider;
		private readonly SRMAProvider _srmaProvider;
		private readonly ILogger<CachedSRMAProvider> _logger;

		public CachedSRMAProvider(ICacheProvider cacheProvider, SRMAProvider srmaProvider, ILogger<CachedSRMAProvider> logger) : base(cacheProvider)
		{
			_cacheProvider = cacheProvider;
			_srmaProvider = srmaProvider;
			_logger = logger;
		}

		public async Task<SRMADto> GetSRMAById(long srmaId)
		{
			var cacheKey = GetCacheKeyForSrmaBySrmaIdKey(srmaId);

			var srma = await GetData<SRMADto>(cacheKey);

			if (srma == null)
			{
				srma = await _srmaProvider.GetSRMAById(srmaId);

				if (srma != null)
				{
					await StoreData<SRMADto>(cacheKey, srma);
				}
			}

			return srma;
		}

		public async Task<SRMADto> SaveSRMA(SRMADto srma)
		{
			var created = await _srmaProvider.SaveSRMA(srma);
			if (created != null)
			{
				UpdateCachedSrma(created);
			}

			return created;
		}

		public async Task<List<SRMADto>> GetSRMAsForCase(long caseUrn)
		{
			var cacheKey = GetCacheKeyForSrmasForCase(caseUrn);

			var srmas = await GetData<List<SRMADto>>(cacheKey);

			if (srmas == null || srmas.Count == 0)
			{
				srmas = await _srmaProvider.GetSRMAsForCase(caseUrn);

				if (srmas?.Count > 0)
				{
					cacheKey = GetCacheKeyForSrmasForCase(srmas.First().CaseUrn);
					await StoreData<List<SRMADto>>(cacheKey, srmas);
				}
			}

			return srmas;
		}

		public async Task<SRMADto> SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			var patched = await _srmaProvider.SetDateAccepted(srmaId, acceptedDate);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}

			return patched;
		}

		public async Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			var patched = await _srmaProvider.SetVisitDates(srmaId, startDate, endDate);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			var patched = await _srmaProvider.SetDateReportSent(srmaId, reportSentDate);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetDateClosed(long srmaId, DateTime? ClosedDate)
		{
			var patched = await _srmaProvider.SetDateClosed(srmaId, ClosedDate);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			var patched = await _srmaProvider.SetOfferedDate(srmaId, offeredDate);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetNotes(long srmaId, string notes)
		{
			var patched = await _srmaProvider.SetNotes(srmaId, notes);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetStatus(long srmaId, SRMAStatus status)
		{
			var patched = await _srmaProvider.SetStatus(srmaId, status);
			if (patched != null)
			{
				if (patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		public async Task SetReason(long srmaId, SRMAReasonOffered reason)
		{
			var patched = await _srmaProvider.SetReason(srmaId, reason);
			if(patched != null)
			{
				if(patched.Id != srmaId)
				{
					throw new InvalidOperationException("Patched SRMA returned from the API is invalid");
				}

				UpdateCachedSrma(patched);
			}
		}

		private async void UpdateCachedSrma(SRMADto srma)
		{
			var cacheKey = GetCacheKeyForSrmaBySrmaIdKey(srma.Id);
			await StoreData<SRMADto>(cacheKey, srma);

			await ClearData(GetCacheKeyForSrmasForCase(srma.CaseUrn));
		}

		private string GetCacheKeyForSrmaBySrmaIdKey(long srmaId)
		{
			return $"SRMA:SRMAId:{srmaId}";
		}

		private static string GetCacheKeyForSrmasForCase(long caseUrn)
		{
			return $"SRMAsForCase:CaseId:{caseUrn}";
		}
	}
}
