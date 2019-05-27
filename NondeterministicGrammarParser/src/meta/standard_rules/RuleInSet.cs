using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleInSet : Rule {
		private string setName;
		private HashSet<string> set;
		

		public RuleInSet(string name, HashSet<string> set) {
			setName = name;
			this.set = set;
			collector = tree => tree.GetAllLeafNodes();
		}
		
		public RuleInSet(RuleManager<string>.NamedSet ns) : this(ns.Name, ns.Set) { }
		

		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			foreach (var parseNode in nodes) {
				if (!set.Contains(parseNode.terminals)) {
					Console.WriteLine(FailReason(parseNode.terminals));
					return false;
				}
			}

			return true;
		}


		public override string FailReason(params string[] args) {
			return $"Terminals ({args[0]} can not be in set {setName}";
		}
	}
}