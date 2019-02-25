using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NondeterminateGrammarParser.parse {
	public abstract class ParseNode {

		public ParseNode parent { get; }
		protected List<ParseNode> children;
		public int intendedChildren { get; set; } = 0;

		public string printAble => printStr(0);

		public string terminals => getTerminals();

		protected ParseNode(ParseNode parent) {
			this.parent = parent;
			parent?.children.Add(this);
			children = new List<ParseNode>();
		}

		public ParseNode[] getChildren() {
			return children.ToArray();
		}

		public abstract bool isEmpty();

		public void clean() {
			for (var i = children.Count - 1; i >= 0; i--) {
				children[i].clean();
				if (children[i].isEmpty()) children.RemoveAt(i);
			}
		}
		
		public virtual void clean(string c) {	
			for (var i = children.Count - 1; i >= 0; i--) {
				children[i].clean(c);
				if (children[i].isEmpty()) children.RemoveAt(i);
			}
		}

		public int Add(ParseNode value) {
			return ((System.Collections.IList) children).Add(value);
		}

		public abstract ParseNode createCopy(ParseNode parent);

		public void print() => print(0);

		private void print(int indent) {
			Console.WriteLine(printStr(indent));
		}

		private string printStr(int indent) {
			string line = "";
			for (int i = 0; i < indent; i++) {
				line += '\t';
			}

			line += this + "\n";
			for (var i = 0; i < children.Count; i++) {
				line += children[i].printStr(indent + 1);
			}

			return line;
		}

		protected abstract string getTerminals();

	}
}