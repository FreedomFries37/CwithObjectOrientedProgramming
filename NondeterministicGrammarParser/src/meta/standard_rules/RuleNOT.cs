using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleNOT : Rule {

		private Rule r;

		public RuleNOT(Rule r) : base(r.collector){
			this.r = r;
		}
		

		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			return !r.GetTruthValue(nodes);
		}
	}
}