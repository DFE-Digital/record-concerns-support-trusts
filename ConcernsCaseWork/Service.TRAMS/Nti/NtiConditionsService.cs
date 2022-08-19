using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiConditionsService : AbstractService, INtiConditionsService
	{
		private readonly ILogger<NtiConditionsService> _logger;

		public NtiConditionsService(IHttpClientFactory httpClientFactory,
			ILogger<NtiConditionsService> logger) : base(httpClientFactory)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiConditionDto>> GetAllConditionsAsync()
		{
			var type1 = new NtiConditionTypeDto
			{
				Id = 1,
				DisplayOrder = 1,
				Name = "Type 1"
			};

			var type2 = new NtiConditionTypeDto
			{
				Id = 2,
				DisplayOrder = 2,
				Name = "Type 2"
			};

			var tmpData = Enumerable.Range(1, 8).Select(x => new NtiConditionDto
			{
				Id = x,
				Name = $"Condition {x}",
				Type = x % 2 == 0 ? type2 : type1,
			}).ToArray();

			return await Task.FromResult(tmpData);
		}
	}
}
