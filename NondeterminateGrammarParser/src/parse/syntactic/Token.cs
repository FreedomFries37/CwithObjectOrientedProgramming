using System.Reflection.Metadata.Ecma335;

namespace NondeterminateGrammarParser.parse.syntactic {
	public class Token : SyntaticObject{

		public string token { get; }

		public Token(string token) {
			this.token = token;
		}

		public override string ToString() {
			return token;
		}

		public override int minimumTerminals() {
			return token.Length;
		}
	}
}