namespace ConcernsCaseWork.Service.Records
{
	public interface IRecordService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(long caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto);
		Task<RecordDto> PatchRecordByUrn(RecordDto recordDto);
	}
}