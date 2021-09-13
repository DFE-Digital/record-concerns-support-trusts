using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public interface IRecordAcademyService
	{
		Task<IList<RecordAcademyDto>> GetRecordsAcademyByRecordUrn(long recordUrn);
		Task<RecordAcademyDto> PostRecordAcademyByRecordUrn(CreateRecordAcademyDto createRecordAcademyDto);
		Task<RecordAcademyDto> PatchRecordAcademyByUrn(RecordAcademyDto recordAcademyDto);
	}
}