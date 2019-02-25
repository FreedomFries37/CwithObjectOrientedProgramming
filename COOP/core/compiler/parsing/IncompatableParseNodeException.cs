using System;

namespace COOP.core.compiler.converters.parsing {
	public class IncompatableParseNodeException : Exception{
		public override string ToString() {
			return "Node type incompatable";
		}

	}
}