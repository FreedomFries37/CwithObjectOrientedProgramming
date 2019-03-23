using System.Collections.Generic;
using COOP.core.structures.v2.exceptions;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.functions.function_bodies;
using COOP.core.structures.v2.global.modifiers;

namespace COOP.core.structures.v2.global.type {
	public class COOPInterface : COOPType{


		public List<COOPAbstract> importedClasses { get; }
		public List<COOPInterface> parentInterfaces { get; }
		protected Dictionary<string, COOPFunction> abstractFunctions;

		public COOPInterface(string name) : base(name) { }
		
		
		public override bool isParent(COOPType type) {
			return isParent(type as COOPInterface);
		}

		public bool isParent(COOPInterface @interface) {
			if (@interface == null) return false;
			if (this ==@interface) return true;
			foreach (COOPInterface parentInterface in parentInterfaces) {
				if (parentInterface.isParent(@interface)) return true;
			}
			return false;
		}

		public virtual bool isUsableFunction(string s, InputList list) {
			if (!abstractFunctions.TryGetValue(s, out COOPFunction function)) return false;
			Body body = function[list];
			if(body == null) throw new MethodDoesNotExistException(s, list);
			FunctionCall functionCall = body.toFunctionCall();
			return getAvailableFunctions(AccessLevel.Private).Contains(functionCall);
		}

		public virtual List<FunctionCall> getAvailableFunctions(AccessLevel accessLevel) {
			List<FunctionCall> output = new List<FunctionCall>();
			foreach (COOPAbstract importedClass in importedClasses) {
				output.AddRange(importedClass.getAvailableFunctions(AccessLevel.Public));
			}
			foreach (COOPInterface parentInterface in parentInterfaces) {
				output.AddRange(parentInterface.getAvailableFunctions(AccessLevel.Protected));
			}
			foreach (COOPFunction functionsValue in abstractFunctions.Values) {
				output.AddRange(functionsValue.getAvailableCalls(accessLevel));
			}
			
			foreach (COOPFunction abstractFunctionsValue in abstractFunctions.Values) {
				output.AddRange(abstractFunctionsValue.getAvailableCalls(accessLevel));
			}

			return output;
		}

		public override bool isStrictlyInterface() => true;
	}
}