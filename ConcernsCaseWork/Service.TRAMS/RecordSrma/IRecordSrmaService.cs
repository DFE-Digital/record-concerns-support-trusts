using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordSrma
{
	public interface IRecordSrmaService
	{
		Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(long recordUrn);
		Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(CreateRecordSrmaDto createRecordSrmaDto);
		Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto);
	}
}