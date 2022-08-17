﻿using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Types
{
	public sealed class TypeCachedService : CachedService, ITypeCachedService
	{
		private readonly ILogger<TypeCachedService> _logger;
		private readonly ITypeService _typeService;

		private const string TypesKey = "Concerns.Types";
		
		public TypeCachedService(ICacheProvider cacheProvider, ITypeService typeService, ILogger<TypeCachedService> logger) 
			: base(cacheProvider)
		{
			_typeService = typeService;
			_logger = logger;
		}
		
		public async Task ClearData()
		{
			await ClearData(TypesKey);
		}
		
		public async Task<IList<TypeDto>> GetTypes()
		{
			_logger.LogInformation("TypeCachedService::GetTypes");
			
			// Check cache
			var types = await GetData<IList<TypeDto>>(TypesKey);
			if (types != null) return types;

			// Fetch from Academies API
			types = await _typeService.GetTypes();

			// Store in cache for 24 hours (default)
			await StoreData(TypesKey, types);
			
			return types;
		}
	}
}