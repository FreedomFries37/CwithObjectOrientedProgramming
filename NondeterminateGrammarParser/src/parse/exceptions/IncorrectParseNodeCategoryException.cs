using System;

namespace NondeterminateGrammarParser.parse.exceptions {
	public class IncorrectParseNodeCategoryException : Exception{
		public IncorrectParseNodeCategoryException(string given, string wanted) : base($"Wanted: {wanted}\tGiven: {given}") { }
	}
}