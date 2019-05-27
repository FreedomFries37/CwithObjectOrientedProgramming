using System.Collections.Generic;
using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta {
	public abstract class Rule {

		protected internal NodeCollector collector { get; set; }

		protected Rule(NodeCollector collector) {
			this.collector = collector;
		}

		protected Rule() { }

		internal abstract bool GetTruthValue(Collection<ParseNode> nodes);

		public bool GetTruthValue(ParseTree t, NodeCollector collector) {
			return GetTruthValue(collector(t));
		}
		
		public bool GetTruthValue(ParseTree t) {
			if (collector == null) return false;
			return GetTruthValue(collector(t));
		}

		public virtual string FailReason(params string[] args) {
			return "Rule Failed";
		}
	}
}