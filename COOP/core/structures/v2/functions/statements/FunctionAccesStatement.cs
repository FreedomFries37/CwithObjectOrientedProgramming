using COOP.core.structures.v2.functions.statements.components;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {
	public class FunctionAccesStatement : Statement{

		public FunctionAccess functionAccess { get; }

		public FunctionAccesStatement(Ownership<COOPFunction> ownership, FunctionAccess functionAccess) : base(ownership) {
			this.functionAccess = functionAccess;
		}

		public override string toUsableToC() {
			return functionAccess.ToString() + ";";
		}

		public override bool validate() {
			throw new System.NotImplementedException();
		}
	}
}