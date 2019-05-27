using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.parse {
	public class ParseTree {

		internal ParseNode head;
		public bool success { get; protected set; }


		public ParseTree(ParseNode head) {
			this.head = head;
			success = true;
		}

		public string terminals => head.terminals;

		public ParseTree[] getChildren() {
			return (from f in head.getChildren() select new ParseTree(f)).ToArray();
		}

		public bool isEmpty() {
			return head.isEmpty();
		}

		public void clean() {
			head.clean();
		}

		public void clean(string c) {
			head.clean(c);
		}

		public int Add(ParseTree value) {
			return head.Add(value.head);
		}

		public ParseTree createCopy(ParseTree parent) {
			return new ParseTree(head.createCopy(parent.head));
		}

		public virtual void print() {
			head.print();
		}

		public bool tryGetChild(string s, out ParseTree find) {
			ParseNode output;
			var getChild = head.tryGetChild(s, out output);
			find = new ParseTree(output);
			return getChild;
		}

		public void Convert(AbstractNodeReTooler reTooler) {
			head.Convert(reTooler);
		}

		public void ConvertAll(AbstractNodeReTooler reTooler) {
			head.ConvertAll(reTooler);
		}

		public Collection<ParseNode> getAllChildrenOfCategory(string s) {
			return new Collection<ParseNode>(head.getAllChildrenOfCategory(s));
		}

		public ParseTree this[string s] => new ParseTree(head[s]);

		public Collection<ParseNode> GetAllLeafNodes() {
			var stack = new Stack<ParseNode>();
			stack.Push(head);
			
			var output = new Collection<ParseNode>();

			while (stack.Count > 0) {
				var n = stack.Pop();
				var parseNodes = n.getChildren();
				if (parseNodes.Length > 0) {
					foreach (var parseNode in parseNodes) {
						stack.Push(parseNode);
					}
				} else {
					output.Add(n);
				}
			}

			return output;
		}
	}
}