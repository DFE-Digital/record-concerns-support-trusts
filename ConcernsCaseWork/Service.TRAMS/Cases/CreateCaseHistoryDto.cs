using System;

namespace Service.TRAMS.Cases
{
	public class CreateCaseHistoryDto
	{
		public DateTimeOffset CreatedAt { get; }
		
		public long CaseUrn { get; }
		
		public string Action { get; }
		
		public string Title { get; }
		
		public string Description { get; }
		
		public CreateCaseHistoryDto(DateTimeOffset createdAt, long caseUrn, string action, string title, string description) => 
			(CreatedAt, CaseUrn, Action, Title, Description) = 
			(createdAt, caseUrn, action, title, description);
	}
}