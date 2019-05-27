using System;

namespace NondeterministicGrammarParser.parse.exceptions {
	public class ChildMissingException : Exception{
		public ChildMissingException(string s) : base($"No such child with category {s}") { }
	}
}