using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class TempNtiWarningLetterReasonsService : INtiWarningLetterReasonsService
	{
		public async Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync()
		{	var testData = Enumerable.Range(1, 8).Select(i => new NtiWarningLetterReasonDto
			{ Id = i, Name = $"Reason {i}", CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5) }).ToList();

			return await Task.FromResult(testData);
		}
	}
}