namespace ConcernsCaseWork.Data.Exceptions;

public class EntityNotFoundException : CaseworkDataException
{
	public int EntityId { get; }
	public string EntityName { get; }
	public EntityNotFoundException(int entityId, string entityName) : base($"{entityName} with id {entityId} not found")
	{
		EntityId = entityId;
		EntityName = entityName;
	}
}