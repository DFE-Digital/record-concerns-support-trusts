using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public interface IRecordSrmaService
	{
		Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(BigInteger recordUrn);
		Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(CreateRecordSrmaDto createRecordSrmaDto);
		Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto);
	}
}