using System.Collections.Generic;

namespace COOP.core.compiler {
	public class ParseNode {
		private string data;
		private List<ParseNode> children;

		public ParseNode(string data) {
			this.data = data;
			children = new List<ParseNode>();
		}

		public string Data => data;

		public void addChild(ParseNode n) {
			children.Add(n);
		}
		
		
	}
}