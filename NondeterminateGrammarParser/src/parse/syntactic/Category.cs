using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
	}
}