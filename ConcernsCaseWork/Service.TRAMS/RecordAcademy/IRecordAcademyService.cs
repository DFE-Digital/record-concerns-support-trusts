using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface IRecordAcademyService
	{
		Task<IList<RecordAcademyDto>> GetRecordsAcademyByRecordUrn(BigInteger recordUrn);
		Task<RecordAcademyDto> PostRecordAcademyByRecordUrn(CreateRecordAcademyDto createRecordAcademyDto);
		Task<RecordAcademyDto> PatchRecordAcademyByUrn(RecordAcademyDto recordAcademyDto);
	}
}