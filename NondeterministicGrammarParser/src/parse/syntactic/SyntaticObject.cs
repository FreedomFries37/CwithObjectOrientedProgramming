using System.Collections.Generic;
using System.Text;

namespace NondeterministicGrammarParser.parse.syntactic {
	public abstract class SyntaticObject {

	
		public abstract int minimumTerminals();
		public virtual bool validate() => true;

		public void print() => print(0, new HashSet<SyntaticObject>());
		public abstract void print(int indent, HashSet<SyntaticObject> visited);

		protected string indent(int size, string itype = "\t") {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < size; i++) {
				builder.Append(itype);
			}
			return builder.ToString();
		}
	}
}