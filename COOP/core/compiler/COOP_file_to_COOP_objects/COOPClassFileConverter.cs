using COOP.core.structures;
using NondeterminateGrammarParser.parse;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public class COOPClassFileConverter : AbstractNodeConverter<COOPClass> {
		
		
		public COOPClassFileConverter() : base(SyntacticCategories.COOPClassFile.name) { }

		protected override COOPClass convertNode(CategoryNode n) {
			throw new System.NullReferenceException();
		}
	}
}