using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using NondeterminateGrammarParser.parse;
using NondeterminateGrammarParser.parse.syntactic;

namespace NondeterminateGrammarParser {
	public class PushdownAutomata {

		public class State {
			public ParseNode head { get; }
			public Stack<SyntaticObject> stack { get; }
			public Queue<string> tokens { get; private set; }
			private int index;

			public string currentToken => tokens.Peek();
			public char currentChar => tokens.Peek()[index];

			public bool completedStack => stack.Count == 0;
			public bool completedTokens => tokens.Count == 0;

			public bool success => completedStack && completedTokens || terminalsLeft() == 0 && minimumTerminalsLeftToComplete() == 0;
			
			public State(ParseNode head, Stack<SyntaticObject> stack, string[] tokens) {
				this.head = head.createCopy(null);
				this.stack = new Stack<SyntaticObject>(stack.Reverse());
				this.tokens = new Queue<string>(tokens);
				index = 0;
			}

			public State(State s) : this(s.head, s.stack, s.tokens.ToArray()) {
				this.index = s.index;
			}

			bool advanceOneChar() {
				if (tokens.Count == 0) return false;
				index++;
				if (currentToken.Length == index) return advanceOneToken();
				return true;
			}

			bool advanceOneToken() {
				if (tokens.Count == 0) return false;
				tokens.Dequeue();
				index = 0;
				return true;
			}

			List<ParseNode> postTraversal(ParseNode h) {
				List<ParseNode> output = new List<ParseNode>();
				foreach (ParseNode parseNode in h.getChildren()) {
					output.AddRange(postTraversal(parseNode));
				}

				output.Add(h);
				return output;
			}

			public ParseNode findNextAvailableNode() {
				var f = postTraversal(head);
				for (var i = 0; i < f.Count; i++) {
					
					ParseNode p = f[i];
					if (p.getChildren().Length < p.intendedChildren) return p;
				}

				return null;
			}

			public int terminalsLeft() {
				return string.Join("", tokens).Substring(index).Length;
			}

			public int minimumTerminalsLeftToComplete() {
				int sum = 0;
				foreach (SyntaticObject syntaticObject in stack) {
					sum += syntaticObject.minimumTerminals();
				}

				return sum;
			}

			public List<State> advanceOneObject() {
				if(completedStack || 
					minimumTerminalsLeftToComplete() > terminalsLeft()
					) return new List<State>();
				//if(completedStack) return new List<State> {this};
				
				SyntaticObject syntaticObject = stack.Pop();
				ParseNode nextAvailable = findNextAvailableNode();
				if (syntaticObject is Terminal) {
					Terminal terminal = syntaticObject as Terminal;
					if (terminal.token == currentChar + "") {
						advanceOneChar();
						new StringNode(nextAvailable as CategoryNode, terminal.token);
						return new List<State> {this };
					}
					return new List<State>();
				}
				if (syntaticObject is Token) {
					Token terminal = syntaticObject as Token;
					if (terminal.token == currentToken) {
						advanceOneToken();
						new StringNode(nextAvailable as CategoryNode, terminal.token);
						return new List<State> {this };
					}
					return new List<State>();
				}
				if (syntaticObject is Category) {
					Category category = syntaticObject as Category;
					List<State> output = new List<State>();
					foreach (SyntaticObject[] syntaticObjects in category) {
						
						
						
						State nextState = new State(this);
						Stack<SyntaticObject> s = nextState.stack;
						for (var i = syntaticObjects.Length - 1; i >= 0; i--) {
							s.Push(syntaticObjects[i]);
						}
						
						new CategoryNode(nextState.findNextAvailableNode(), category, syntaticObjects.Length);
						nextState.index = index;
						output.Add(nextState);
					}

					return output;
				}

				return null;
			}
			
			
			public override string ToString() {
				return $"{nameof(stack)}: {string.Join("", stack)}, {nameof(tokens)}: {string.Join("", tokens).Substring(index)}";
			}
		}

		private string[] delimiters { get; set; }
		private string originalString { get; set; }

		private Category start { get; set; }

		public PushdownAutomata(string[] delimiters, string originalString, Category start) {
			this.delimiters = delimiters;
			this.originalString = originalString;
			this.start = start;
		}

		public ParseNode parse() {
			string[] tokens = split(originalString);
			List<State> stateSet = new List<State>();
			
			foreach (SyntaticObject[] syntaticObjects in start) {
				ParseNode head = new CategoryNode(null, start, syntaticObjects.Length);
				stateSet.Add(new State(head, new Stack<SyntaticObject>(syntaticObjects.Reverse()), tokens));
			}

			while (stateSet.Count > 0 && !oneSuccess(stateSet)) {
				List<State> nextStateSet = new List<State>();

				foreach (State state in stateSet) {
					nextStateSet.AddRange(state.advanceOneObject());
				}

				stateSet = nextStateSet;
			}

			if (stateSet.Count > 0) {
				foreach (State state in stateSet) {
					if (state.success) {
						
						state.head.clean();
						return state.head;
					}
				}
			}

			return null;
		}

		private bool oneSuccess(List<State> list) {
			foreach (State state in list) {
				if (state.success) return true;
			}

			return false;
		}

		public string[] split(string s) {
			List<string> output = new List<string> {s};
			for (var i = 0; i < delimiters.Length; i++) {
				string delim = delimiters[i];
				for (var j = 0; j < output.Count; j++) {
					string w = output[j];
					string[] split = w.Split(delim);
					output.RemoveAt(j);
					output.InsertRange(j, split);
					j += split.Length - 1;
				}
			}

			return output.ToArray();
		}

		
	}
}