using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiReasonsService : INtiReasonsService
	{
		private readonly ILogger<NtiReasonsService> _logger;

		public NtiReasonsService(ILogger<NtiReasonsService> logger)
		{
			_logger = logger;
		}

		public async Task<ICollection<NtiReasonDto>> GetNtiReasonsAsync()
		{
			var tempData = Enumerable.Range(1, 5).Select(i => new NtiReasonDto
			{
				Id = i,
				CreatedAt = DateTime.Now,
				Name = $"Reason {i}"
			});

			return await Task.FromResult(tempData.ToArray());
		}
	}
}
