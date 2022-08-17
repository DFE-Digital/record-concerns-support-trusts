using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Types;
using ConcernsCasework.Service.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Types
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
		public async Task<TypeModel> GetTypeModel()
		{
			_logger.LogInformation("TypeModelService::GetTypeModel");
			
			var typesDto = await GetTypes();

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

		public async Task<TypeModel> GetSelectedTypeModelByUrn(long urn)
		{
			_logger.LogInformation("TypeModelService::GetSelectedTypeModelByUrn");

			var typesDto = await GetTypes();
			var typeModel = await GetTypeModel();

			var selectedTypeDto = typesDto.FirstOrDefault(t => t.Urn.CompareTo(urn) == 0) ?? typesDto.First();
			typeModel.Type = selectedTypeDto.Name ?? string.Empty;
			typeModel.SubType = selectedTypeDto.Description ?? string.Empty;

			return typeModel;
		}
		
		public async Task<TypeModel> GetTypeModelByUrn(long urn)
		{
			_logger.LogInformation("TypeModelService::GetTypeModelByUrn");

			var typesDto = await GetTypes();
			
			return TypeMapping.MapDtoToModel(typesDto, urn);
		}

		public async Task<IList<TypeDto>> GetTypes()
		{
			_logger.LogInformation("TypeModelService::GetTypes");
			
			var typesDto = await _typeCachedService.GetTypes();

			return typesDto;
		}
	}
}