using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class TempNtiWarningLetterStatusesService : INtiWarningLetterStatusesService
	{
		public async Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync()
		{
			var testData = Enumerable.Range(1, 3).Select(i => new NtiWarningLetterStatusDto
			{ Id = i, Name = $"Status {i}", CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5) }).ToList();

			return await Task.FromResult(testData);
		}
	}
}
