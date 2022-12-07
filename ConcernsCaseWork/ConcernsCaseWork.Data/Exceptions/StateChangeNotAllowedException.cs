namespace ConcernsCaseWork.Data.Exceptions;

// Should be used when an operation on an object is not allowable due to its state.
// For example, an entity in a closed state may not be closed.
public class StateChangeNotAllowedException : CaseworkDataException
{
	public StateChangeNotAllowedException(string message) : base(message)
	{

	}
}