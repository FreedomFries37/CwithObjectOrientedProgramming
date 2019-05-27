using System.Collections.Generic;
using System.Linq;
using System.Text;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.parse {
	public class FailureParseTree : ParseTree {

		private class MultiParseNode : ParseNode {
			
			public List<ParseNode> childNodes { get; }

			public MultiParseNode(List<ParseNode> childNodes) : base(null) {
				this.childNodes = childNodes;
			}

			public override bool isEmpty() {
				return childNodes.Count == 0;
			}

			public override ParseNode createCopy(ParseNode parent) {
				var copy = new List<ParseNode>();
				foreach (ParseNode childNode in childNodes) {
					copy.Add(childNode.createCopy(this));
				}
				
				return new MultiParseNode(copy);
			}

			protected override string getTerminals() {
				if (isEmpty()) return "";
				StringBuilder output = new StringBuilder();
				
				foreach (ParseNode childNode in childNodes) {
					
					
					if (!isEmpty()) {
						string current = childNode.terminals;
						
						output.Append(current);
					}
				}

				return output.ToString();
			}

			public override void Convert(AbstractNodeReTooler reTooler) {
				ConvertAll(reTooler);
			}
		}
		
		public readonly List<ParseTree> trees;

		public FailureParseTree(List<ParseTree> trees) : base(new MultiParseNode((from f in trees select f.head).ToList())) {
			this.trees = trees;
			success = false;
		}

		public override void print() {
			foreach (ParseTree parseTree in trees) {
				parseTree.print();
				
			}
		}
	}
}