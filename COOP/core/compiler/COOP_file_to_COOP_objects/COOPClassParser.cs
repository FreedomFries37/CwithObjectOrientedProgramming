using System.Collections.Generic;
using System.Data;
using System.IO;
using NondeterministicGrammarParser;
using NondeterministicGrammarParser.meta;
using NondeterministicGrammarParser.meta.standard_rules;
using NondeterministicGrammarParser.parse;
using NondeterministicGrammarParser.parse.syntactic;
using Rule = NondeterministicGrammarParser.meta.Rule;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public class COOPClassParser {

		private Category startSymbol { get; }
		public List<List<State>> history { get; private set; }

		private RuleManager<string> RuleManager;

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
			RuleManager = new RuleManager<string>();


			RuleManager.AddSet("reserved", ReservedWords.getReservedWords());
			RuleManager.AddSet("reserved_types", "int", "double", "float", "char", "long");
			Rule class_name_rule = new RuleAND(
				
			);
		}

		public ParseTree parse(string w) {
			PushDownAutomata a = new PushDownAutomata(Deliminators, w, startSymbol);
			a.Manager = RuleManager;
			
			var parseTree = a.parse();
			history = a.history;
			return parseTree;
		}
		
		public ParseTree parseFile(string path) {
			string w = File.ReadAllText(path);
			return parse(w);
		}
	}
}