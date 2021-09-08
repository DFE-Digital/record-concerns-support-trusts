using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordSrma
{
	public interface IRecordSrmaService
	{
		Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(string recordUrn);
		Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(RecordSrmaDto recordSrmaDto);
		Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto);
	}
}