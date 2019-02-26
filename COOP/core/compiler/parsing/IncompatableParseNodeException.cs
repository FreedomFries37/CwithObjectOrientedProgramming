using System;

namespace COOP.core.compiler.parsing {
	public class IncompatableParseNodeException : Exception{
		public override string ToString() {
			return "Node type incompatable";
		}

	}
}