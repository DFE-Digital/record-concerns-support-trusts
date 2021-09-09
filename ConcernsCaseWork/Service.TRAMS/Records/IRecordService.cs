using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public interface IRecordService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(BigInteger caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto);
		Task<RecordDto> PatchRecordByUrn(UpdateRecordDto updateRecordDto);
	}
}