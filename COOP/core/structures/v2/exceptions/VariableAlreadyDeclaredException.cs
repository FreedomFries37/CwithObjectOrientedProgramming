using System;

namespace COOP.core.structures.v2.exceptions {
	public class VariableAlreadyDeclaredException : Exception{
		public VariableAlreadyDeclaredException(string message) : base($"{message} already declared.") { }
	}
}