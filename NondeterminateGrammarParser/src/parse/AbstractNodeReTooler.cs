using NondeterminateGrammarParser.parse.exceptions;
using NondeterminateGrammarParser.parse.syntactic;

namespace NondeterminateGrammarParser.parse {
	public abstract class AbstractNodeReTooler{

		public string InputCategory { get; }

		protected AbstractNodeReTooler(string inputCategory) {
			InputCategory = inputCategory;
		}

		public void retool(CategoryNode node) {
			if(node.category.name != InputCategory) throw new IncorrectParseNodeCategoryException(node.category.ToString(), InputCategory);
			retoolNode(node);
		}

		public abstract void retoolNode(CategoryNode node);
	}
}