using System.IO;
using NondeterminateGrammarParser;
using NondeterminateGrammarParser.parse;
using NondeterminateGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public class COOPClassParser {

		private Category startSymbol { get; }

		private static readonly string[] Deliminators = {
			"}",
			"{",
			"(",
			")",
			".",
			",",
			"[",
			"]",
			"\"",
			"\'",
			";",
			"*",
			"/",
			"+",
			"-",
			"=",
			"^",
			"&",
			"|"
		};
		
		
		public COOPClassParser() {
			startSymbol = SyntacticCategories.start;
		}

		public ParseTree parse(string w) {
			PushdownAutomata a = new PushdownAutomata(Deliminators, w, startSymbol);
			return a.parse();
		}
		
		public ParseTree parseFile(string path) {
			string w = File.ReadAllText(path);
			PushdownAutomata a = new PushdownAutomata(Deliminators, w, startSymbol);
			return a.parse();
		}
	}
}