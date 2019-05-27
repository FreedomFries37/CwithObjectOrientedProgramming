using NondeterministicGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public partial class SyntacticCategories {
		
		public static Category symbolStartChar { get; } = new Category("symbol_start_char") {new Terminal('_') };
		public static Category symbolCont { get; } = new Category("symbol_cont");
		
		public static Category symbol { get;} = new Category("symbol") {{symbolStartChar, symbolCont}};
		public static Category symbolContTail { get; }  = new Category("symbol_cont_tail") {symbolCont};
		public static Category symbolContChar { get; } = new Category("symbol_cont_char") {new Terminal('_')};
		
		public static Category stringEscapeChar { get; } = 
			new Category("string_escape_char") {
				new Terminal('n'),
				new Terminal('t'),
				new Terminal('r'),
				new Terminal('0'),
				new Terminal('v'),
				new Terminal('"'),
				new Terminal('/')
			};

		public static Category stringtail { get; } = new Category("string_tail");
		public static Category @string { get; } = new Category("string") {{new Terminal('"'), stringtail, new Terminal('"')}};
		public static Category stringchar { get; } = new Category("string_char") {{new Terminal('\\'), stringEscapeChar}};
		
		public static Category digit { get; } = new Category("digit");
		
		public static Category integerTail { get; } = new Category("integer_tail", true);
		public static Category @integer { get; } = new Category("integer") {{digit, integerTail}};

		public static Category floatingPoint { get; } = new Category("floating_point") {{integer, ".", integer}};
		
		public static Category @float { get; } = new Category("float") {{floatingPoint, "l"}, {floatingPoint, "L"}};
		public static Category @double { get; } = new Category("double") {{floatingPoint, "d"}, {floatingPoint, "D"}};

		public static Category number { get; } = new Category("number") {integer, @float, @double};

		public static Category literal { get; } = new Category("literal") {number, @string};
	}
}