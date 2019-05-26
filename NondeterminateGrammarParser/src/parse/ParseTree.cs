using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;

namespace NondeterminateGrammarParser.parse {
	public class ParseTree {

		internal ParseNode head;


		public ParseTree(ParseNode head) {
			this.head = head;
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

		public void print() {
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

		public Collection<ParseTree> getAllChildrenOfCategory(string s) {
			return new Collection<ParseTree>((from f in head.getAllChildrenOfCategory(s) select new ParseTree(f)).ToArray());
		}

		public ParseTree this[string s] => new ParseTree(head[s]);
		
	}
}