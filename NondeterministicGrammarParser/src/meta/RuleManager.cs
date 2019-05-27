using System.Collections;
using System.Collections.Generic;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta {
	public class RuleManager<TSetType> : IRuleManager {

		public class NamedSet {
			public string Name { get; }
			public HashSet<TSetType> Set { get; }

			internal NamedSet(string name, HashSet<TSetType> set) {
				Name = name;
				Set = set;
			}

			public NamedSet(string name) {
				Name = name;
				Set = new HashSet<TSetType>();
			}

			protected bool Equals(NamedSet other) {
				return string.Equals(Name, other.Name) && Equals(Set, other.Set);
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((NamedSet) obj);
			}

			public override int GetHashCode() {
				unchecked {
					return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Set != null ? Set.GetHashCode() : 0);
				}
			}
		}

		private Ruleset ruleset { get; }
		private Dictionary<string, NamedSet> sets;

		public RuleManager() {
			ruleset = new Ruleset();
			sets = new Dictionary<string, NamedSet>();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<Rule> GetEnumerator() {
			return ruleset.GetEnumerator();
		}

		public bool ConstraintsHold(ParseTree t) {
			if (ruleset.Count == 0) return true;
			return ruleset.TrueForAll(x => x.GetTruthValue(t));
		}

		public bool ConstraintsHold(ParseNode t) {
			return ConstraintsHold(new ParseTree(t));
		}

		public bool AddRule(Rule r) {
			if (ruleset.Contains(r)) return false;
			ruleset.Add(r);
			return true;
		}

		public bool RemoveRule(Rule r) {
			if (!ruleset.Contains(r)) return false;
			ruleset.Remove(r);
			return true;
		}

		public bool AddSet(string name, params TSetType[] insert) {
			if (sets.ContainsKey(name)) return false;
			var namedSet = new NamedSet(name);
			sets.Add(name, namedSet);
			foreach (TSetType i in insert) {
				namedSet.Set.Add(i);
			}

			return true;
		}

		public bool AddToSet(string name, params TSetType[] insert) {
			if (sets.ContainsKey(name)) return false;
			var namedSet = GetSet(name);
			
			foreach (TSetType i in insert) {
				namedSet.Set.Add(i);
			}

			return true;
		}

		public NamedSet GetSet(string name) {
			return sets[name];
		}
	}
}