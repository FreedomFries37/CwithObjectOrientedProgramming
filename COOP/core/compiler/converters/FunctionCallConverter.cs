using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Transactions;
using COOP.core.compiler.converters;
using COOP.core.compiler.converters.parsing;
using COOP.core.inheritence;

namespace COOP.core.compiler {
	public class FunctionCallConverter : IConverter<string, FunctionCallConvertedInformation> {
		
		private readonly Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName;
		private readonly Dictionary<NameInputTypePair, bool>   originalNameAndInputTypesToisStatic;
		private readonly Dictionary<string, COOPClass>         functionToReturnType;
		private readonly Dictionary<string, COOPClass> availableClasses;
		private readonly Dictionary<string, COOPClass> variablesToType;

		private Dictionary<string, bool> isClassDict;

		public COOPClass parentClass { get; set; }
		public ClassHierarchy hierarchy { get; set; }

		private static readonly Regex SYMBOL_REGEX;


		static FunctionCallConverter() {
			SYMBOL_REGEX = new Regex(@"[_a-zA-Z]\w*");
			
		}

		public FunctionCallConverter(
			Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName, 
			Dictionary<NameInputTypePair, bool> originalNameAndInputTypesToisStatic, 
			Dictionary<string, COOPClass> functionToReturnType,
			List<COOPClass> availableClasses,
			Dictionary<string, COOPClass> vars) {
			this.originalNameAndInputTypesToMangledName = originalNameAndInputTypesToMangledName;
			this.originalNameAndInputTypesToisStatic = originalNameAndInputTypesToisStatic;
			this.functionToReturnType = functionToReturnType;
			this.availableClasses = new Dictionary<string, COOPClass>();
			variablesToType = vars;
			isClassDict = new Dictionary<string, bool>();
			foreach (COOPClass availableClass in availableClasses) {
				isClassDict.Add(availableClass.Name, true);
				this.availableClasses.Add(availableClass.Name, availableClass);
			}
		}
		

		public Collection<FunctionCallConvertedInformation> convert(string functionCall, ClassHierarchy hierarchy) {
			

			Parser parser = new Parser(functionCall, Parser.ReadOptions.STRING);
			ParseTree parseTree = new ParseTree(parser.ParseObject);
			if (!parseTree.successfulParse) return null;
			
			string output = fixCall(parseTree.Head);
			
			return new Collection<FunctionCallConvertedInformation> { new FunctionCallConvertedInformation(output) };
		}

		class CallReturn {
			public string fixedString { get; }
			public COOPClass resultingReturnType { get; }

			public CallReturn(string fixedString, COOPClass resultingReturnType) {
				this.fixedString = fixedString;
				this.resultingReturnType = resultingReturnType;
			}
		}

		public string fixCall(ParseNode n) {
			return calculateObject(n)?.fixedString;
		}

		private CallReturn calculateObject(ParseNode o) {
			if (!o.Equals("<object>")) return null;

			CallChain c = new CallChain(originalNameAndInputTypesToMangledName,
				originalNameAndInputTypesToisStatic,
				functionToReturnType,
				availableClasses,
				variablesToType,
				isClassDict,
				parentClass,
				hierarchy);

			CallChain.CallNode n = c.createCallNodeChain(o);
			Console.WriteLine(n);
			COOPClass returnType = n.type;
			string fixedForC = c.fixForC(n);
			return new CallReturn(fixedForC, returnType);
		}

		

		private bool isClass(string name) {
			return isClassDict.ContainsKey(name);
		}

		private COOPClass calculateLowestMemberCoopClass(ParseNode o, COOPClass p) {
			if (!o.Equals("<member>")) return null;
			if (!o.Contains("<member>")) return variablesToType[o.terminals];

			ParseNode ptr = o;
			COOPClass current = p;
			while (ptr.Contains("<member>")) {
				
				string memberName = ptr["<symbol>"].terminals;
				if (o.Contains("<function_call>")) {
					current = current.Functions[memberName].ReturnType;
				} else {
					current = current.VarNames[memberName];
				}

				ptr = ptr["<member>"];
			}

			return current;
		}
	}
}
