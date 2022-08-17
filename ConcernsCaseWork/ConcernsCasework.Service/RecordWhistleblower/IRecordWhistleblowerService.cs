namespace ConcernsCasework.Service.RecordWhistleblower
{
	public interface IRecordWhistleblowerService
	{
		Task<IList<RecordWhistleblowerDto>> GetRecordsWhistleblowerByRecordUrn(long recordUrn);
		Task<RecordWhistleblowerDto> PostRecordWhistleblowerByRecordUrn(CreateRecordWhistleblowerDto createRecordWhistleblowerDto);
		Task<RecordWhistleblowerDto> PatchRecordWhistleblowerByUrn(RecordWhistleblowerDto recordWhistleblowerDto);
	}
}