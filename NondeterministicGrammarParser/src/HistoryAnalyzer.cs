using System;
using System.Collections.Generic;
using System.Text;

namespace NondeterministicGrammarParser {
	public class StateAnalyzer {

		public State head { get; }

		public StateAnalyzer(State head) {
			this.head = head;
		}

		public List<State> History() {
			List<State> output = new List<State>();

			State c = head;
			while (c != null) {
				output.Insert(0, c);
				c = c.previousState;
			}
			
			return output;
		}
	}

	public class HistoryAnalyzer {

		private PushDownAutomata pda;

		public HistoryAnalyzer(PushDownAutomata pda) {
			this.pda = pda;
		}

		public class StateNode {
			public State state { get; }

			public string Name {
				get {
					string output;
					
					if (parent == null) {
						output = "" + childNum;
					}else if (parent.children.Count == 1) {
						output = parent.Name;
					} else {
						output = parent.Name + '.' + childNum;
					}



					return output;
				}
			}

			private StateNode parent;
			public List<StateNode> children { get; }
			private int childNum;

			internal StateNode(StateNode parent, int childNum, State state) {
				this.parent = parent;
				parent?.children.Add(this);
				this.state = state;
				this.childNum = childNum;
				children = new List<StateNode>();
			}

			internal StateNode(State state) : this(null, 1, state) { }

			public void print() {
				Console.WriteLine(printAsString());
			}

			public string printAsString() {
				Queue<(int layer, StateNode node)> states = new Queue<(int, StateNode)>();
				states.Enqueue((0, this));
				StringBuilder output = new StringBuilder();
				
				int currentLevel = 0;
				while (states.Count > 0) {
					(int nextlevel, StateNode node) = states.Dequeue();
					if (nextlevel > currentLevel) {
						currentLevel = nextlevel;
						output.Append("\n");
					}
					output.Append(node.childNum + "  ");
					foreach (var nodeChild in node.children) {
						states.Enqueue((nextlevel + 1, nodeChild));
					}
				}
				output.Append("\n");
				return output.ToString();
			}
		}

		public StateNode HistoryTree() {
			Dictionary<State, StateNode> stateNodes = new Dictionary<State, StateNode>();
			List<List<State>> pdaHistory = pda.history;
			bool init = true;
			StateNode head = null;
			foreach (List<State> states in pdaHistory) {
				if (init) {
					var first = states[0];
					head = new StateNode(first);
					stateNodes.Add(first, head);
					init = false;
				} else {
					foreach (State state in states) {
						StateNode parent = stateNodes[state.previousState];
						int childNum = parent.children.Count + 1;
						StateNode next = new StateNode(parent, childNum, state);
						stateNodes.Add(state, next);
					}
				}
			}


			return head;
		}
	}
}