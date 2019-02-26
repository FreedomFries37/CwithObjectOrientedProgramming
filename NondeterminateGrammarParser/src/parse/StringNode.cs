using System.Collections.Generic;

namespace NondeterminateGrammarParser.parse {
	public class StringNode : ParseNode{

		public string value { get; }

		public StringNode(CategoryNode parent, string value) : base(parent) {
			this.value = value;
		}

		public override bool isEmpty() {
			return value == "";
		}

		public override ParseNode createCopy(ParseNode parent) {
			return new StringNode(parent as CategoryNode, value);
		}

		public override string ToString() {
			return value;
		}

		protected override string getTerminals() {
			return value;
		}

		public override void Convert(AbstractNodeReTooler reTooler) {
			
		}
	}
}