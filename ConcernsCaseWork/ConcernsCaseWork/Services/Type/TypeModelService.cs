using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Type
{
	public sealed class TypeModelService : ITypeModelService
	{
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<TypeModelService> _logger;

		public TypeModelService(ITypeCachedService typeCachedService, ILogger<TypeModelService> logger)
		{
			_typeCachedService = typeCachedService;
			_logger = logger;
		}
		
		/// <summary>
		/// Key -> type name
		/// Value -> class encapsulates urn and subtype
		/// </summary>
		/// <returns></returns>
		public async Task<TypeModel> GetTypes()
		{
			_logger.LogInformation("TypeModelService::GetTypes");
			
			var typesDto = await _typeCachedService.GetTypes();

			var typesDictionary = new Dictionary<string, IList<TypeModel.TypeValueModel>>();
			
			foreach (var typeDto in typesDto)
			{
				if (typesDictionary.ContainsKey(typeDto.Name) && typesDictionary.TryGetValue(typeDto.Name, out var subTypes))
				{
					subTypes.Add(new TypeModel.TypeValueModel{ Urn = typeDto.Urn, SubType = typeDto.Description });
				}
				else
				{
					typesDictionary.Add(typeDto.Name, new List<TypeModel.TypeValueModel> { new TypeModel.TypeValueModel{ Urn = typeDto.Urn, SubType = typeDto.Description } });
				}
			}

			return new TypeModel { TypesDictionary = typesDictionary };
		}
	}
}