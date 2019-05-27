using System.Collections.Generic;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta {
	public interface IRuleManager : IEnumerable<Rule> {
		
		bool ConstraintsHold(ParseTree t);
		bool ConstraintsHold(ParseNode t);

		bool AddRule(Rule r);
		bool RemoveRule(Rule r);
		
		
	}
}