using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NondeterminateGrammarParser.parse.syntactic {
	public class Category : SyntaticObject, IEnumerable<SyntaticObject[]> {

		public string name { get; }
		private SyntaticObject[][] rules;

		public Category(SyntaticObject[][] rules, string name) {
			this.rules = rules;
			this.name = name;
		}

		public Category(string name) {
			rules = new SyntaticObject[0][];
			this.name = name;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
 
		public IEnumerator<SyntaticObject[]> GetEnumerator() {
			return rules.ToList().GetEnumerator();
		}

		public void Add(params SyntaticObject[] o) {
			var fixedRules = new SyntaticObject[rules.Length+1][];
			Array.Copy(rules, 0, fixedRules, 0, rules.Length);
			fixedRules[fixedRules.Length - 1] = o;
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
	}
}