using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public interface IRecordAcademyService
	{
		Task<IList<RecordAcademyDto>> GetRecordsAcademyByRecordUrn(string recordUrn);
		Task<RecordAcademyDto> PostRecordAcademyByRecordUrn(RecordAcademyDto recordAcademyDto);
		Task<RecordAcademyDto> PatchRecordAcademyByUrn(RecordAcademyDto recordAcademyDto);
	}
}