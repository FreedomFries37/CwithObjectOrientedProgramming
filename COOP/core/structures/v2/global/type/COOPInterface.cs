using System;
using System.Collections.Generic;
using COOP.core.structures.v2.exceptions;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.functions.function_bodies;
using COOP.core.structures.v2.global.modifiers;

namespace COOP.core.structures.v2.global.type {
	
	[Obsolete("This implementation of interface won't work under current implementation")]
	public class COOPInterface : COOPType{


		
		public List<COOPInterface> parentInterfaces { get; }
		

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

		

		public override bool isStrictlyInterface() => true;
	}
}