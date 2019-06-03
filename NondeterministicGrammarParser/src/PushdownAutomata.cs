using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NondeterministicGrammarParser.meta;
using NondeterministicGrammarParser.parse;
using NondeterministicGrammarParser.parse.syntactic;

namespace NondeterministicGrammarParser {
	public class PushDownAutomata {
		private string[] delimiters { get; set; }
		private string originalString { get; set; }

		private Category start { get; set; }

		public List<List<State>> history { get; private set; }
		public RuleManager<string> Manager { get; set; }

		public PushDownAutomata(string[] delimiters, string originalString, Category start) {
			this.delimiters = delimiters;
			this.originalString = originalString;
			this.start = start;
			Manager = new RuleManager<string>();
		}

		public ParseTree parse() {
			history = new List<List<State>>();
			string[] tokens = split(originalString);
			List<State> stateSet = new List<State>();

			var objects = new Stack<SyntaticObject>();
			objects.Push(start);
			State startState = new State(null, objects, tokens);
			addToHistory(new List<State> {startState});
			
			foreach (SyntaticObject[] syntaticObjects in start) {
				ParseNode head = new CategoryNode(null, start, syntaticObjects.Length);
				stateSet.Add(new State(head, new Stack<SyntaticObject>(syntaticObjects.Reverse()), tokens));
			}

			while (stateSet.Count > 0 && !oneSuccess(stateSet)) {
				addToHistory(stateSet);

				List<State> nextStateSet = new List<State>();

				foreach (State state in stateSet) {
					List<State> nextStateFromThisState = state.advanceOneObject();
					
					for (var i = nextStateFromThisState.Count - 1; i >= 0; i--) {
						
						if (!StateHolds(nextStateFromThisState[i])) {
							nextStateFromThisState.RemoveAt(i);
						}
					}
					
					nextStateSet.AddRange(nextStateFromThisState);
				}

				stateSet = nextStateSet;
			}
			addToHistory(stateSet);

			List<ParseTree> fails = new List<ParseTree>();
			if (stateSet.Count > 0) {
				
				foreach (State state in stateSet) {
					
					if (state.success) {
						
						state.head.clean();
						return new ParseTree(state.head);
					}
					
					fails.Add(new ParseTree(state.head));
					
				}
				
				
			}

			return new FailureParseTree(fails);
		}

		private bool StateHolds(State s) {
			return Manager.ConstraintsHold(s.head);
		}

		private bool oneSuccess(List<State> list) {
			foreach (State state in list) {
				if (state.success) return true;
			}

			return false;
		}

		public string[] split(string s) {
			List<string> output = Regex.Split(s, "\\s+").ToList();
			
			for (var i = 0; i < delimiters.Length; i++) {
				string delim = delimiters[i];
				for (var j = 0; j < output.Count;) {
					string w = output[j];
					List<string> split = w.Split(delim).ToList();
					for (var k = split.Count - 1; k > 0; k--) {
						split.Insert(k, delim);
					}
					output.RemoveAt(j);
					output.InsertRange(j, split);
					j += split.Count;
				}
			}

			output.RemoveAll(x => x == "");
			return output.ToArray();
		}

		private void addToHistory(List<State> states) {
			var copiedList = new List<State>(from f in states select new State(f));
			history.Add(copiedList);
		}
		
	}
}