using System;
using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta {

	public delegate Collection<ParseNode> NodeCollector(ParseTree t);
}