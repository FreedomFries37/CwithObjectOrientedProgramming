using System.Linq.Expressions;
using COOP.core.structures.v2.exceptions;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {


	public class VarDeclaration : Statement, IDeclaration {


		public COOPType type { get; }
		public string name { get; }

		public VarDeclaration(Ownership<COOPFunction> ownership, COOPType type, string name) : base(ownership) {
			this.type = type;
			this.name = name;
		}

		public override bool validate() {
			throw new System.NotImplementedException();
		}

		public override string toUsableToC() {
			throw new System.NotImplementedException();
		}

		public COOPType getVarType() {
			return type;
		}

		public string getVarName() {
			return name;
		}
	}
	
}