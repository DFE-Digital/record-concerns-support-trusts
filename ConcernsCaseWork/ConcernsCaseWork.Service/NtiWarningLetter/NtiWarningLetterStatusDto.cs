namespace ConcernsCaseWork.Service.NtiWarningLetter
{
	public class NtiWarningLetterStatusDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsClosingState { get; set; }
		public string PastTenseName { get; set; }


		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
