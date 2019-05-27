using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleOutOfSet : Rule{
		private string setName;
		private HashSet<string> set;
		private string category;

		public RuleOutOfSet(string name, HashSet<string> set) {
			setName = name;
			this.set = set;
			collector = tree => tree.GetAllLeafNodes();
		}
		
		public RuleOutOfSet(RuleManager<string>.NamedSet ns) : this(ns.Name, ns.Set) { }
		
		
		
		
		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			foreach (var parseNode in nodes) {
				if (set.Contains(parseNode.terminals)) {
					Console.WriteLine(FailReason(parseNode.terminals));
					return false;
				}
			}

			return true;
		}
		
		
		
		public override string FailReason(params string[] args) {
			return $"Terminals ({args[0]} of {category} not be in set {setName}";
		}
	}
}