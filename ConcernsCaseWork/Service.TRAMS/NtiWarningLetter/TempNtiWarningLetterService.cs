using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public class TempNtiWarningLetterService : INtiWarningLetterService
	{
		private List<NtiWarningLetterDto> _records;

		public TempNtiWarningLetterService()
		{
			_records = new List<NtiWarningLetterDto>();
		}
		public Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter)
		{
			newNtiWarningLetter.Id = _records.Select(r => r.Id).DefaultIfEmpty().Max() + 1;
			_records.Add(newNtiWarningLetter);
			return Task.FromResult(newNtiWarningLetter);
		}

		public Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId)
		{
			var wl = _records?.SingleOrDefault(w => w.Id == ntiWarningLetterId);
			return Task.FromResult(wl);
		}

		public async Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn)
		{
			var wls = _records?.Where(w => w.CaseUrn == caseUrn).ToList();
			return await Task.FromResult(wls);
		}

		public Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter)
		{
			_records = _records?.Where(w => w.Id != ntiWarningLetter.Id).ToList();
			return CreateNtiWarningLetterAsync(ntiWarningLetter);
		}
	}
}
