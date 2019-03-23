using System;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.exceptions {
	public class MethodDoesNotExistException : Exception{
		public MethodDoesNotExistException(string name, InputList list) : base($"Method {name} with inputs {list} does not exist.") { }
	}
}