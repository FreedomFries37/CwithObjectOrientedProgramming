namespace COOP.core.compiler.parsing {
	public partial class Parser {


		public bool ParseObject(out ParseNode output) {
			output = new ParseNode("<object>");
			if (!ParseObject(output)) return false;
			output = output["<object>"];
			return true;
		}
		
		public bool ParseFunctionCall(out ParseNode output) {
			output = new ParseNode("<function_call>");
			if (!ParseCaller(output)) return false;
			if (!ParseMember(output)) return false;

			return true;
		}

		public bool ParseMember(ParseNode parent) {
			var next = new ParseNode("<member>");

			if (!ConsumeChar('.')) return false;
			if (!ParseSymbol(next)) return false;
			if (MatchChar('(')) {
				if (!ParseFunctionCall(next)) return false;
			}

			if (MatchChar('.')) {
				if (!ParseMember(next)) return false;
			}
			
			parent.AddChild(next);
			return true;
		}
		
		public bool ParseFunctionCall(ParseNode output) {
			var next = new ParseNode("<function_call>");
			
			if (!ConsumeChar('(')) return false;
			if (!ParseList(next, ParseObject, @"\s*,\s*")) return false;
			if (!ConsumeChar(')')) return false;
			

			output.AddChild(next);
			return true;
		}

		public bool ParseObject(ParseNode parent) {
			var next =  new ParseNode("<object>");

			if (!ParseSymbol(next)) return false;
			if (MatchChar('(')) {
				if (!ParseFunctionCall(next)) return false;
			}
			if (MatchChar('.')) {
				if (!ParseMember(next)) return false;
			}

			parent.AddChild(next);
			return true;
		}

		private bool ParseCaller(ParseNode parent) {
			var next =  new ParseNode("<caller>");
			if (!ParseSymbol(next)) return false;
			
			parent.AddChild(next);
			return true;
		}


		private bool ParseSymbol(ParseNode parent) {
			var next = new ParseNode("<symbol>");

			if (!ParseSymbolStart(next)) return false;
			if (MatchPattern(@"\w")) {
				if (!ParseSymbolBack(next)) return false;
			}

			parent.AddChild(next);
			return true;
		}

		private bool ParseSymbolStart(ParseNode parent) {
			ParseNode next = new ParseNode("<symbol_start>");
			
			if (!MatchPattern(@"[a-zA-Z_]")) return false;
			ParseNode nextNext = new ParseNode("" + CurrentCharacter);
			
			next.AddChild(nextNext);
			AdvancePointer();
			
			parent.AddChild(next);
			return true;
		}

		private bool ParseSymbolChar(ParseNode parent) {
			ParseNode next = new ParseNode("<symbol_char>");
			
			if (!MatchPattern(@"\w")) return false;
			ParseNode nextNext = new ParseNode("" + CurrentCharacter);
			
			next.AddChild(nextNext);
			AdvancePointer();
			
			parent.AddChild(next);
			return true;
		}

		private bool ParseSymbolBack(ParseNode parent) {
			ParseNode next = new ParseNode("<symbol_back>");

			if (!ParseSymbolChar(next)) return false;
			if (!ParseSymbolBackTail(next)) return false;
			
			parent.AddChild(next);
			return true;
		}

		private bool ParseSymbolBackTail(ParseNode parent) {
			ParseNode next = new ParseNode("<symbol_back_tail>");

			if (MatchPattern(@"\w")) {
				if (!ParseSymbolBack(next)) return false;
			}
			
			parent.AddChild(next);
			return true;
		}

	}
}