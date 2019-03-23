using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using COOP.core.compiler.parsing;
using COOP.core.inheritence;
using COOP.core.structures;
using COOP.core.structures.v1;

namespace COOP.core.compiler.converters {
	public class CallChain {
		
		private readonly Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName;
		private readonly Dictionary<NameInputTypePair, bool>   originalNameAndInputTypesToisStatic;
		private readonly Dictionary<string, COOPClass>         functionToReturnType;
		private readonly Dictionary<string, COOPClass>         availableClasses;
		private readonly Dictionary<string, COOPClass>         variablesToType;

		private Dictionary<string, bool> isClassDict;

		private COOPClass parentClass;
		private ClassHierarchy hierarchy;
		

		public abstract class CallNode {
			public COOPClass type { get; }
			

			public CallNode(COOPClass type) {
				this.type = type;
			}

			
		}

		

		public class SymbolNode : CallNode{

			public string symbol { get; }

			public SymbolNode(COOPClass type, string symbol) : base(type) {
				this.symbol = symbol;
			}

			public override string ToString() {
				return $"{symbol}";
			}

			
		}
		
		public class ClassCallNode : SymbolNode {
			public ClassCallNode(COOPClass type) : base(type, "this") { }

			public override string ToString() {
				return type.Name;
			}
			
			
		}

		public class CastCallNode : ObjectFunctionCallNode {

			public CastCallNode(COOPClass type, CallNode parentObject) : base(type, $"cast<{type}>", parentObject) {
				
			}

			public override string ToString() {
				return $"(({type}) {parentObject})";
			}
		}

		public abstract class FunctionCallNode : SymbolNode {

			public List<CallNode> parameters { get; }

			public FunctionCallNode(COOPClass type, string symbol) : base(type, symbol) {
				parameters = new List<CallNode>();
			}

			protected string ParamsToString() {
				string output = "(";
				foreach (CallNode callNode in parameters) {
					output += callNode + ",";
				}

				if(output.EndsWith(','))
					output = output.Remove(output.Length - 1);
				return output + ")";
			}
			
			
		}

		public class StaticFunctionCallNode : FunctionCallNode {
			public COOPClass @class { get; }

			public StaticFunctionCallNode(COOPClass type, string symbol, COOPClass @class) : base(type, symbol) {
				this.@class = @class;
			}

			public override string ToString() {
				return @class.Name + base.ToString() + ParamsToString();
			}

			
		}

		public class ObjectFunctionCallNode : FunctionCallNode {

			public CallNode parentObject { get; }

			public ObjectFunctionCallNode(COOPClass type, string symbol, CallNode parentObject) : base(type, symbol) {
				this.parentObject = parentObject;
			}

			public override string ToString() {
				return parentObject.ToString() + "." + base.ToString() + ParamsToString();
			}
		}


		public CallChain(Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName, Dictionary<NameInputTypePair, bool> originalNameAndInputTypesToisStatic, Dictionary<string, COOPClass> functionToReturnType, Dictionary<string, COOPClass> availableClasses, Dictionary<string, COOPClass> variablesToType, Dictionary<string, bool> isClassDict, COOPClass parentClass, ClassHierarchy hierarchy) {
			this.originalNameAndInputTypesToMangledName = originalNameAndInputTypesToMangledName;
			this.originalNameAndInputTypesToisStatic = originalNameAndInputTypesToisStatic;
			this.functionToReturnType = functionToReturnType;
			this.availableClasses = availableClasses;
			this.variablesToType = variablesToType;
			this.isClassDict = isClassDict;
			this.parentClass = parentClass;
			this.hierarchy = hierarchy;
		}


		public CallNode createCallNodeChain(ParseNode o) {
			if (!o.Equals("<object>")) return null;

			bool hasFunctionCall = o.Contains("<function_call>"),
				hasMember = o.Contains("<member>"),
				isStatic = hasFunctionCall && parentClass.Functions[o["<symbol>"].terminals].IsStatic;

			CallNode output;

			if (hasFunctionCall) {
				if (isStatic) {
					output = createStaticFunctionCall(o["<symbol>"], o["<function_call>"]);
				} else {
					output = createObjectFunctionCall(new ClassCallNode(parentClass), o["<symbol>"],
						o["<function_call>"]);
				}
			} else {
				output = createSymbolNode(o["<symbol>"]);
			}

			if (hasMember) {
				output = convertMember(output, o["<member>"]);
			}

			return output;
		}

		private StaticFunctionCallNode createStaticFunctionCall(ParseNode s, ParseNode f) {
			if (!s.Equals("<symbol>") || !f.Equals("<function_call>")) return null;
			COOPClass type = functionToReturnType[s.terminals];
			StaticFunctionCallNode output = new StaticFunctionCallNode(
				type,
				s.terminals,
				parentClass
			);

			foreach (ParseNode node in Parser.ConvertListNodeToListOfListObjects(f["<list_object>"])) {
				output.parameters.Add(createCallNodeChain(node));
			}

			return output;
		}
		
		private ObjectFunctionCallNode createObjectFunctionCall(CallNode parent, ParseNode s, ParseNode f) {
			if (!s.Equals("<symbol>") || !f.Equals("<function_call>")) return null;
			COOPClass type = getReturnTypeForFunction(parent.type, s.terminals);
			ObjectFunctionCallNode output = new ObjectFunctionCallNode(
				type,
				s.terminals,
				parent
				);

			foreach (ParseNode node in Parser.ConvertListNodeToListOfListObjects(f["<list_object>"])) {
				output.parameters.Add(createCallNodeChain(node));
			}

			return output;
		}

		private CallNode convertMember(CallNode parent, ParseNode m) {
			if (!m.Equals("<member>")) return null;


			bool hasFunctionCall = m.Contains("<function_call>"),
				hasMember = m.Contains("<member>");

			CallNode output;
			
			if (hasFunctionCall) {
				output = createObjectFunctionCall(parent, m["<symbol>"], m["<function_call>"]);
			} else {
				output = createSymbolNode(parent, m["<symbol>"]);
			}

			if (hasMember) {
				output = convertMember(output, m["<member>"]);
			}

			return output;
		}

		private SymbolNode createSymbolNode(ParseNode s) {
         			if (!s.Equals("<symbol>")) return null;
         
         			return new SymbolNode(variablesToType[s.terminals], s.terminals);
         }
		
		private SymbolNode createSymbolNode(CallNode parent, ParseNode s) {
			if (!s.Equals("<symbol>")) return null;

			return new SymbolNode(parent.type.VarNames[s.terminals], s.terminals);
		}
		
		private bool isClass(string name) {
			return isClassDict.ContainsKey(name);
		}

		public string fixForC(CallNode callNode) {
			string output = "";
			if (callNode is FunctionCallNode) {
				
				var fixedNode = (FunctionCallNode) callNode;
				List<COOPClass> inputs = new List<COOPClass>();
				
				foreach (CallNode parameter in fixedNode.parameters) {
					inputs.Add(parameter.type);
				}
				
				

				string parameters = "(";
				if (callNode is ObjectFunctionCallNode) {
					
					ObjectFunctionCallNode node = callNode as ObjectFunctionCallNode;
					
					
					inputs.Insert(0, node.parentObject.type);
					
					NameInputTypePair tempPair = new NameInputTypePair(fixedNode.symbol, inputs);
					while (getMangeledName(tempPair) == null) {
						inputs[0] = hierarchy.getParent(inputs[0]);
						if (inputs[0] == null) {
							return null;
						}
						tempPair = new NameInputTypePair(fixedNode.symbol, inputs);
					}
					
					
					parameters += fixForC(node.parentObject);
					if (inputs.Count > 1) parameters += ",";
				}
				NameInputTypePair pair = new NameInputTypePair(fixedNode.symbol, inputs);
				for (var i = 0; i < fixedNode.parameters.Count; i++) {
					parameters += fixForC(fixedNode.parameters[i]);
					if (i < fixedNode.parameters.Count - 1) parameters += ",";
				}

				parameters += ")";
				string mangled = originalNameAndInputTypesToMangledName[pair];

				output = mangled + parameters;
			} else if (callNode is SymbolNode) {
				output = callNode.ToString();
			}

			return output;
		}

		private string getMangeledName(NameInputTypePair pair) {
			if (originalNameAndInputTypesToMangledName.ContainsKey(pair))
				return originalNameAndInputTypesToMangledName[pair];
			return null;
		}

		private COOPClass getReturnTypeForFunction(COOPClass original, string name) {
			COOPClass c = original;
			COOPClass output = null;
			while (output == null) {

				if (c.Functions.TryGetValue(name, out COOPFunction f)) {
					output = f.ReturnType;
					
				}

				if (hierarchy.getParent(c) != null)
					c = hierarchy.getParent(c);
				else break;
			}


			return output;
		}
	}
}