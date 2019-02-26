using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using NondeterminateGrammarParser.parse.exceptions;
using NondeterminateGrammarParser.parse.syntactic;

namespace NondeterminateGrammarParser.parse {
	public class CategoryNode : ParseNode{

		public Category category { get; }


		public CategoryNode(ParseNode parent, Category category, int intendedChildren) : base(parent) {
			this.category = category;
			this.intendedChildren = intendedChildren;
		}

		public override bool isEmpty() {
			bool output = true;
			foreach (ParseNode parseNode in getChildren()) {
				if (!parseNode.isEmpty()) {
					output = false;
					break;
				}
			}

			return output;
		}

		public override void clean(string c) {
			if (c == category.name) {
				string t = terminals;
				children = new List<ParseNode> { new StringNode(this, t)};
			}else base.clean(c);
		}

		public override ParseNode createCopy(ParseNode parent) {
			CategoryNode output = new CategoryNode(parent as CategoryNode, category, intendedChildren);
			foreach (ParseNode parseNode in getChildren()) {
				parseNode.createCopy(output);
			}

			return output;
		}

		public override void Convert(AbstractNodeReTooler reTooler) {
			try {
				reTooler.retool(this);
			}
			catch (IncorrectParseNodeCategoryException) { }
		}

		public override string ToString() {
			return category.ToString();
		}

		protected override string getTerminals() {
			string output = "";
			foreach (ParseNode parseNode in getChildren()) {
				output += parseNode.terminals;
			}

			return output;
		}
	}
}