using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface IRecordWhistleblowerService
	{
		Task<IList<RecordWhistleblowerDto>> GetRecordsWhistleblowerByRecordUrn(BigInteger recordUrn);
		Task<RecordWhistleblowerDto> PostRecordWhistleblowerByRecordUrn(CreateRecordWhistleblowerDto createRecordWhistleblowerDto);
		Task<RecordWhistleblowerDto> PatchRecordWhistleblowerByUrn(RecordWhistleblowerDto recordWhistleblowerDto);
	}
}