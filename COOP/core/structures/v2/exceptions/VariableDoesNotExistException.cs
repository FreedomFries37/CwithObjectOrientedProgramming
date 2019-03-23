using System;

namespace COOP.core.structures.v2.exceptions {
	public class VariableDoesNotExistException : Exception{
		public VariableDoesNotExistException(string message) : base($"{message} not a variable.") { }

	}
}