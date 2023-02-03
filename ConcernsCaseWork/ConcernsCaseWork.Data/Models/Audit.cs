namespace ConcernsCaseWork.Data.Models;

public class Audit
{
	public Audit(string entityName, string userName, DateTimeOffset timeOfChange, AuditChangeType changeType, string newValues)
	{
		EntityName = entityName;
		UserName = userName;
		TimeOfChange = timeOfChange;
		ChangeType = changeType;
		NewValues = newValues;
	}

	public int Id { get; init; }
	public string EntityName { get; init; }
	public string UserName { get; init; }
	public DateTimeOffset TimeOfChange { get; init; }
	public AuditChangeType ChangeType { get; init; }
	public string NewValues { get; init; }
}