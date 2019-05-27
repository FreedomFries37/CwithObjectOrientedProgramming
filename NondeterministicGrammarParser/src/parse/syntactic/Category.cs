using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NondeterministicGrammarParser.parse.syntactic {
	public class Category : SyntaticObject, IEnumerable<SyntaticObject[]>, IEquatable<Category> {

		public string name { get; }
		public bool StrictTokenUsage { get; set; } = false;
		private SyntaticObject[][] rules;
		

		public Category(SyntaticObject[][] rules, string name) {
			this.rules = rules;
			this.name = name;
		}

		public Category(string name, bool generateEmptyRule = false) {
			rules = new SyntaticObject[0][];
			this.name = name;
			if (generateEmptyRule) Add();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
 
		public IEnumerator<SyntaticObject[]> GetEnumerator() {
			return rules.ToList().GetEnumerator();
		}

		public void Add(SyntaticObject o1, params SyntaticObject[] o2) {
			var fixedRules = new SyntaticObject[rules.Length+1][];
			Array.Copy(rules, 0, fixedRules, 0, rules.Length);
			var o = new SyntaticObject[o2.Length + 1];
			o[0] = o1;
			Array.Copy(o2,0, o, 1, o2.Length);
			fixedRules[fixedRules.Length - 1] = o;
			rules = fixedRules;
		}

		public void Add() {
			var fixedRules = new SyntaticObject[rules.Length+1][];
			Array.Copy(rules, 0, fixedRules, 0, rules.Length);
			fixedRules[fixedRules.Length - 1] = new SyntaticObject[0];
			rules = fixedRules;
		}
		
		public void Add(object oS, params object[] o2) {
			var fixedRules = new SyntaticObject[rules.Length+1][];
			Array.Copy(rules, 0, fixedRules, 0, rules.Length);
			List<SyntaticObject> list = new List<SyntaticObject>();
			var o = new object[o2.Length + 1];
			o[0] = oS;
			Array.Copy(o2,0, o, 1, o2.Length);
			foreach (object o1 in o) {
				if (o1 is string) {
					list.Add(new Token(o1 as string));
				}else if (o1 is SyntaticObject) {
					list.Add(o1 as SyntaticObject);
				}else if (o1 is char) {
					list.Add(new Terminal((char) o1));
				}
			}
			fixedRules[fixedRules.Length - 1] = list.ToArray();
			rules = fixedRules;
		}

		public override string ToString() {
			return "<" + name + ">";
		}

	

		public override int minimumTerminals() {
			
			foreach (SyntaticObject[] syntaticObjects in rules) {
				if (syntaticObjects.Length == 0) return 0;
			}
			int min = int.MaxValue;
			for (var i = 0; i < rules.Length; i++) {
				int sum = 0;
				for (var j = 0; j < rules[i].Length; j++) {
					sum += rules[i][j].minimumTerminals();
				}

				if (sum < min) min = sum;
			}

			return min;
		}

		public void addTerminalRulesForRange(char lower, char higher) {
			for (char i = lower; i <= higher; i++) {
				Add(new Terminal(i));
			}
		}

		public override bool validate() {
			return validate(new HashSet<SyntaticObject>());
		}

		private bool validate(HashSet<SyntaticObject> visited) {
			visited.Add(this);

			foreach (SyntaticObject[] syntaticObjects in rules) {
				foreach (SyntaticObject syntaticObject in syntaticObjects) {
					Category category = syntaticObject as Category;
					if (category != null) {
						if (!visited.Contains(syntaticObject)) {
							if (!(syntaticObject as Category).validate(visited)) {
								Console.WriteLine($"{this} invalid");
								return false;
							}
						}
					} else {
						visited.Add(syntaticObject);
					}
				}
			}

			return true;
		}

		public bool Equals(Category other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(rules, other.rules) && string.Equals(name, other.name);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Category) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((rules != null ? rules.GetHashCode() : 0) * 397) ^ (name != null ? name.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Category left, Category right) {
			return Equals(left, right);
		}

		public static bool operator !=(Category left, Category right) {
			return !Equals(left, right);
		}

		public override void print(int indent, HashSet<SyntaticObject> visited) {
			Console.WriteLine(this.indent(indent) + this);
			bool old = visited.Contains(this);
			if (!old) {
				visited.Add(this);
				for (var i = 0; i < rules.Length; i++) {
					Console.WriteLine(this.indent(indent + 1) + $"Rule {i}:");
					
					foreach (SyntaticObject syntaticObject in rules[i]) {
						syntaticObject.print(indent + 1, visited);
					}
					
					Console.WriteLine();
				}
			}
		}
	}
}