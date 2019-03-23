using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type.included;

namespace COOP.core.structures.v2.global.type {
	public class COOPClass : COOPAbstract, CConvertable {
		

		public Dictionary<string, COOPFunction> functions { get; }
		public string cName { get; set; }



		public COOPClass(string name, COOPClass super) : base(name, super) {
			
			functions = new Dictionary<string, COOPFunction>();
			cName = $"struct {name}";
		}
		
		public COOPClass(string name) : this(name, IncludedClasses.Object) { }

		public string toUsableToC() {
			return cName;
		}

		public override bool isStrictlyInterface() => false;

		public override bool isStrictlyAbstract() => false;

		public override bool isStrictlyClass() => true;

		public override List<FunctionCall> getAvailableFunctions(AccessLevel accessLevel) {
			var output = base.getAvailableFunctions(accessLevel);
			foreach (COOPFunction abstractFunctionsValue in functions.Values) {
				output.AddRange(abstractFunctionsValue.getAvailableCalls(accessLevel));
			}

			return output;
		}

		public Field[] getExternFields() {
			Field[] allFields = getFields(AccessLevel.Sealed, AccessLevelRule.inLessThanEqualTo);
			return (from f in allFields where f.modifiers.isExternallyImplemented select f).ToArray();
		}
		
		public bool allFunctionsImplemented() {


			return true;
		}
	}
}