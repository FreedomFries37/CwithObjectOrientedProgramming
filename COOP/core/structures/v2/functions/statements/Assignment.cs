using System.Threading.Tasks.Sources;
using COOP.core.structures.v2.functions.function_bodies;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {
	public class Assignment : Statement, IAssignment{

		public string variable { get; }
		public COOPObject coopObject { get; }

		private VariableInformation variableInformation;

		public Assignment(Ownership<COOPFunction> ownership, VariableInformation variableInformation, string variable, COOPObject coopObject) : base(ownership) {
			this.variableInformation = variableInformation;
			this.variable = variable;
			this.coopObject = coopObject;
		}

		public override bool validate() {
			return true;
		}

		public override string toUsableToC() {
			return "";
		}

		public string getVarName() {
			return variable;
		}

		public COOPObject getCOOPObject() {
			return coopObject;
		}

		public string getCOOPObjectString() {
			throw new System.NotImplementedException();
		}
	}
}