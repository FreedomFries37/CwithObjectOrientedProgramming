using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using NondeterminateGrammarParser.parse.exceptions;
using NondeterminateGrammarParser.parse.syntactic;

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

		public bool tryGetChild(string s, out ParseNode find) {
			try {
				find = this[s];
				return true;
			}
			catch (ChildMissingException e) {
				find = null;
				return false;
			}
		}

		public abstract void Convert(AbstractNodeConverter converter);

		public void ConvertAll(AbstractNodeConverter converter) {
			Convert(converter);
			foreach (ParseNode parseNode in children) {
				parseNode.Convert(converter);
			}
		}

		public Collection<ParseNode> getAllChildrenOfCategory(string s) {
			var output = new List<ParseNode>();
			foreach (ParseNode parseNode in children) {
				
				if(parseNode is CategoryNode && ((CategoryNode) parseNode).tryGetChild(s, out ParseNode found)) output.Add(found);
				output.AddRange(parseNode.getAllChildrenOfCategory(s));
			}

			return new Collection<ParseNode>(output);
		}

		public ParseNode this[string s]{
			get {
				foreach (CategoryNode parseNode in from f in children where f is CategoryNode select f as CategoryNode) {
					if (parseNode.category.name == s) return parseNode;
				}

				throw new ChildMissingException(s);
			}
		}
	}
}