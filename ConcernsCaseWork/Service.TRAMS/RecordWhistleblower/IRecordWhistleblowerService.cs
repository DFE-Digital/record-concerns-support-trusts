using Service.TRAMS.Trusts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordWhistleblower
{
	public interface IRecordWhistleblowerService
	{
		Task<IList<RecordWhistleblowerDto>> GetRecordsWhistleblowerByRecordUrn(string recordUrn);
		Task<RecordWhistleblowerDto> PostRecordWhistleblowerByRecordUrn(RecordWhistleblowerDto recordWhistleblowerDto);
		Task<RecordWhistleblowerDto> PatchRecordWhistleblowerByUrn(RecordWhistleblowerDto recordWhistleblowerDto);
	}
}