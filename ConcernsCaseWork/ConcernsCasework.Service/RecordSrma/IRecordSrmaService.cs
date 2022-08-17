namespace ConcernsCasework.Service.RecordSrma
{
	public interface IRecordSrmaService
	{
		Task<IList<RecordSrmaDto>> GetRecordsSrmaByRecordUrn(long recordUrn);
		Task<RecordSrmaDto> PostRecordSrmaByRecordUrn(CreateRecordSrmaDto createRecordSrmaDto);
		Task<RecordSrmaDto> PatchRecordSrmaByUrn(RecordSrmaDto recordSrmaDto);
	}
}