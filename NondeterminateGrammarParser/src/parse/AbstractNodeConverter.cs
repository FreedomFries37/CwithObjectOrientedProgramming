using NondeterminateGrammarParser.parse.exceptions;
using NondeterminateGrammarParser.parse.syntactic;

namespace NondeterminateGrammarParser.parse {
	public abstract class AbstractNodeConverter{

		public string InputCategory { get; }

		protected AbstractNodeConverter(string inputCategory) {
			InputCategory = inputCategory;
		}

		public void convert(CategoryNode node) {
			if(node.category.name != InputCategory) throw new IncorrectParseNodeCategoryException(node.category.ToString(), InputCategory);
			convertNode(node);
		}

		public abstract void convertNode(CategoryNode node);
	}
}