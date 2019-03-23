using System.Collections.Generic;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements.components {
	public class FunctionAccess {

		public FunctionCall functionCall { get; }
		public List<COOPObject> inputObjects;
		public COOPClass parentClass;

		public FunctionAccess(List<COOPObject> inputObjects, COOPClass parentClass, FunctionCall functionCall) {
			this.inputObjects = inputObjects;
			this.parentClass = parentClass;
			this.functionCall = functionCall;
		}
	}
}