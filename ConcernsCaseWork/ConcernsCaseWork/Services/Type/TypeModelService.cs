using Service.Redis.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Type
{
	public sealed class TypeModelService : ITypeModelService
	{
		private readonly ITypeCachedService _typeCachedService;

		public TypeModelService(ITypeCachedService typeCachedService)
		{
			_typeCachedService = typeCachedService;
		}
		
		public async Task<IDictionary<string, IList<string>>> GetTypes()
		{
			var typesDto = await _typeCachedService.GetTypes();
			var typesDic = new Dictionary<string, IList<string>>();
			
			foreach (var typeDto in typesDto)
			{
				if (typesDic.ContainsKey(typeDto.Name) && typesDic.TryGetValue(typeDto.Name, out var subTypes))
				{
					subTypes.Add(typeDto.Description);
				}
				else
				{
					typesDic.Add(typeDto.Name, string.IsNullOrEmpty(typeDto.Description) ? new List<string>() : new List<string> { typeDto.Description });
				}
			}

			return typesDic;
		}
	}
}