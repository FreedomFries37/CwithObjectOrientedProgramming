using System;
using System.Collections.Generic;
using System.Linq;
using NondeterministicGrammarParser.parse;
using NondeterministicGrammarParser.parse.syntactic;

namespace NondeterministicGrammarParser {
	public class State {
		public ParseNode head { get; }
		public State previousState { get; }
		public Stack<SyntaticObject> stack { get; }
		public Queue<string> tokens { get; private set; }
		private int index;

		public string currentToken => tokens.Peek();
		public char currentChar => tokens.Peek()[index];

		public bool completedStack => stack.Count == 0;
		public bool completedTokens => tokens.Count == 0;

		public bool success => completedStack && completedTokens || terminalsLeft() == 0 && minimumTerminalsLeftToComplete() == 0;

	
		
		
		public State(ParseNode head, Stack<SyntaticObject> stack, string[] tokens) {
			this.head = head?.createCopy(null);
			this.stack = new Stack<SyntaticObject>(stack.Reverse());
			this.tokens = new Queue<string>(tokens);
			index = 0;
			previousState = null;
			
		}

		public State(State s) : this(s.head, s.stack, s.tokens.ToArray()) {
			this.index = s.index;
			previousState = s;
			
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
			return String.Join("", tokens).Substring(index).Length;
		}

		public int minimumTerminalsLeftToComplete() {
			int sum = 0;
			foreach (SyntaticObject syntaticObject in stack) {
				sum += syntaticObject.minimumTerminals();
			}

			return sum;
		}

		public List<State> advanceOneObject() {
			if(completedStack
				//				|| minimumTerminalsLeftToComplete() > terminalsLeft()
			) return new List<State>();
			//if(completedStack) return new List<State> {this};
				
			SyntaticObject syntaticObject = stack.Pop();
			
			State nextState = new State(this);
			ParseNode nextAvailable = nextState.findNextAvailableNode();
			if (syntaticObject is Terminal) {
				Terminal terminal = syntaticObject as Terminal;
				if (terminal.token == currentChar + "") {
				
					nextState.advanceOneChar();
					new StringNode(nextAvailable as CategoryNode, terminal.token);
					return new List<State> {nextState };
				}
				return new List<State>();
			}
			if (syntaticObject is Token) {
				Token terminal = syntaticObject as Token;
				
				if (terminal.token == currentToken) {
					
					nextState.advanceOneToken();
					new StringNode(nextAvailable as CategoryNode, terminal.token);
					return new List<State> {nextState };
				}
				return new List<State>();
			}
			if (syntaticObject is Category) {
				Category category = syntaticObject as Category;
				List<State> output = new List<State>();
					
				if (!category.StrictTokenUsage) {
					foreach (SyntaticObject[] syntaticObjects in category) {



						nextState = new State(this);
						Stack<SyntaticObject> s = nextState.stack;
						for (var i = syntaticObjects.Length - 1; i >= 0; i--) {
							s.Push(syntaticObjects[i]);
						}

						new CategoryNode(nextState.findNextAvailableNode(), category, syntaticObjects.Length);
						nextState.index = index;
						output.Add(nextState);
					}
				} else {
					
					PushDownAutomata internalDriver = new PushDownAutomata(new string[0], nextState.currentToken, category);
					ParseTree tokenParse = internalDriver.parse();
						
					if (tokenParse != null && tokenParse.success) {
						nextState.findNextAvailableNode().Add(tokenParse);
						nextState.advanceOneToken();
						output.Add(nextState);
					}
				}

				return output;
			}

			return null;
		}
			
			
		public override string ToString() {
			return $"{nameof(stack)}: {String.Join("", stack)}, {nameof(tokens)}: {String.Join("", tokens).Substring(index)}";
		}

		
	}
}