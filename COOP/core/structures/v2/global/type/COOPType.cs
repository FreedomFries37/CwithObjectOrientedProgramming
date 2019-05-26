using System.Collections.Generic;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.global.modifiers;

namespace COOP.core.structures.v2.global.type {
	public abstract class COOPType {
		public string Name { get; }

		public COOPType(string name) {
			Name = name;
		}
		
		public List<COOPAbstract> importedClasses { get; }
		protected Dictionary<string, COOPFunction> abstractFunctions;

		public virtual string defaultValue() {
			return "NULL";
		}

		public abstract bool isParent(COOPType type);

		public virtual bool isStrictlyInterface() => false;
		public virtual bool isStrictlyAbstract() => false;
		public virtual bool isStrictlyClass() => false;
		
		public virtual List<FunctionCall> getAvailableFunctions(AccessLevel accessLevel) {
			List<FunctionCall> output = new List<FunctionCall>();
			foreach (COOPAbstract importedClass in importedClasses) {
				output.AddRange(importedClass.getAvailableFunctions(AccessLevel.Public));
			}
			/*
			foreach (COOPInterface parentInterface in parentInterfaces) {
				output.AddRange(parentInterface.getAvailableFunctions(AccessLevel.Protected));
			}
			*/
			foreach (COOPFunction functionsValue in abstractFunctions.Values) {
				output.AddRange(functionsValue.getAvailableCalls(accessLevel));
			}
			
			foreach (COOPFunction abstractFunctionsValue in abstractFunctions.Values) {
				output.AddRange(abstractFunctionsValue.getAvailableCalls(accessLevel));
			}

			return output;
		}

		public override string ToString() {
			return $"COOPType: {Name}";
		}
	}
}