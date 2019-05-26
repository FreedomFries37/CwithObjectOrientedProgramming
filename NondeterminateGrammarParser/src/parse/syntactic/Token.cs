using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;

namespace NondeterminateGrammarParser.parse.syntactic {
	public class Token : SyntaticObject, IEquatable<Token> {

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

		public bool Equals(Token other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(token, other.token);
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj) || obj is Token other && Equals(other);
		}

		public override int GetHashCode() {
			return token.GetHashCode();
		}

		public static bool operator ==(Token left, Token right) {
			return Equals(left, right);
		}

		public static bool operator !=(Token left, Token right) {
			return !Equals(left, right);
		}

		public override void print(int indent, HashSet<SyntaticObject> set) {
			Console.WriteLine(this.indent(indent) + this);
		}
	}
}