using System;

namespace ConcernsCaseWork.Exceptions
{
	/// <summary>
	/// This introduced to use the existing exception based error propegation without accidentaly exposing 
	/// unrelated InvalidOperationExceptions to the user
	/// </summary>
	public class InvalidUserInputException : Exception
	{
		public InvalidUserInputException() : base()
		{ }

		public InvalidUserInputException(string msg) : base(msg)
		{ }
	}
}
