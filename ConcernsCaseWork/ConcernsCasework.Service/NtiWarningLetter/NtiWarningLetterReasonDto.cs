namespace ConcernsCasework.Service.NtiWarningLetter
{
	public class NtiWarningLetterReasonDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
