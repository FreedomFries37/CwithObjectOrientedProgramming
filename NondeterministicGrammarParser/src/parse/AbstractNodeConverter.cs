using NondeterministicGrammarParser.parse.exceptions;

namespace NondeterministicGrammarParser.parse {
	public abstract class AbstractNodeConverter<T> {

		public string convertCategory { get; }

		protected AbstractNodeConverter(string convertCategory) {
			this.convertCategory = convertCategory;
		}

		public T convert(CategoryNode n) {
			if(n.category.name != convertCategory) throw new IncorrectParseNodeCategoryException(n.category.name, convertCategory);
			return convertNode(n);
		}

		protected abstract T convertNode(CategoryNode n);
	}
	
}