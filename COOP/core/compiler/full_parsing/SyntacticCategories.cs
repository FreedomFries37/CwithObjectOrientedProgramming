using System.Linq.Expressions;
using NondeterminateGrammarParser.parse.syntactic;

namespace COOP.core.compiler.full_parsing {
	public class SyntacticCategories {

		public static Category symbol { get;}
		public static Category symbolStartChar { get; }
		public static Category symbolCont { get; }
		public static Category symbolContTail { get; }
		public static Category symbolContChar { get; }
		public static Category @string { get; }
		public static Category stringchar { get; }
		public static Category stringEscapeChar { get; }
		public static Category stringtail { get; }
		
		public static Category objectMemberAccess { get; } //guaranteed to end in member
		
		public static Category objectFunction { get; } //guaranteed to end in function

		public static Category objectAccess { get; } //ends in either
		
		



		static SyntacticCategories() {
			
			symbolContChar = new Category("symbol_cont_char") {new Terminal('_')};
			symbolContChar.addTerminalRulesForRange('a', 'z');
			symbolContChar.addTerminalRulesForRange('A', 'A');
			symbolContChar.addTerminalRulesForRange('0', 'Z');

			symbolCont = new Category("symbol_cont");
			symbolContTail = new Category("symbol_cont_tail") {symbolCont};
			
			symbolCont.Add(symbolContChar, symbolContTail);
			symbolCont.Add();
			
			symbolStartChar = new Category("symbol_start_char") {new Terminal('_') };
			symbolStartChar.addTerminalRulesForRange('a', 'z');
			symbolStartChar.addTerminalRulesForRange('A', 'Z');
			
			symbol = new Category("symbol") {{symbolStartChar, symbolCont}};

			stringEscapeChar = new Category("string_escape_char") {
				new Terminal('n'),
				new Terminal('t'),
				new Terminal('r'),
				new Terminal('0'),
				new Terminal('v'),
				new Terminal('"')
			};
			
			
			stringchar = new Category("string_char") {{new Terminal('\\'), stringEscapeChar}};
			stringchar.addTerminalRulesForRange(' ', '!');
			stringchar.addTerminalRulesForRange('#', '[');
			stringchar.addTerminalRulesForRange(']', '~');
			
			
			stringtail = new Category("string_tail");
			stringtail.Add();
			stringtail.Add(stringchar, stringtail);
			
			@string = new Category("string") {{new Terminal('"'), stringtail, new Terminal('"')}};
			
		}

		public Category start { get; } = symbol;
		
		

	}
}