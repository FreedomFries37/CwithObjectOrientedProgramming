using System.Collections.Generic;
using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleOR : Rule{
		
		private Rule[] rules;
		private bool useIndividual;
		private Dictionary<Rule, NodeCollector> collectors;

		public RuleOR(NodeCollector collector, params Rule[] rules) : base(collector) {
			this.rules = rules;
		}
		
		public RuleOR(params Rule[] rules) {
			this.rules = rules;
			useIndividual = true;
			collectors = new Dictionary<Rule, NodeCollector>();
			foreach (var rule in rules) {
				collectors.Add(rule, rule.collector);
			}
			
		}

		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			foreach (Rule rule in rules) {
				
				if (rule.GetTruthValue(nodes)) {
					return true;
				}
			}

			return false;
		}

		public new bool GetTruthValue(ParseTree t) {

			if (!useIndividual) return GetTruthValue(collector(t));
			
			foreach (Rule rule in rules) {
				
				if (rule.GetTruthValue(collectors[rule](t))) {
					return true;
				}
			}

			return false;
		}
	}
}