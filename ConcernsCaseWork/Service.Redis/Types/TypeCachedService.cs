using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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

			// Fetch from TRAMS API
			types = await _typeService.GetTypes();

			// Store in cache for 24 hours (default)
			await StoreData(TypesKey, types);
			
			return types;
		}

		public async Task<TypeDto> GetTypeByNameAndDescription(string name, string description)
		{
			_logger.LogInformation("TypeCachedService::GetTypeByNameAndDescription");
			
			var types = await GetTypes();
			
			return types.FirstOrDefault(t => 
				t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
				(string.IsNullOrEmpty(t.Description) || t.Description.Equals(description, StringComparison.OrdinalIgnoreCase)));
		}
	}
}