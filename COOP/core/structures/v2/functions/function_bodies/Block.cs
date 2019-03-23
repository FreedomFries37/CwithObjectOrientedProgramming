using System.Collections;
using System.Collections.Generic;
using COOP.core.structures.v2.functions.statements;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.function_bodies {
	public class Block : Statement, IEnumerable<Statement> {

		private VariableInformation variableInformation;
		private COOPFunction ownerFunction => variableInformation.ownership.owner.ownership.owner;
		private COOPClass ownerClass => ownerFunction.ownership.owner;

		private List<Statement> statements;


		public Block(Ownership<COOPFunction> ownership, VariableInformation variableInformation, List<Statement> statements) : base(ownership) {
			this.variableInformation = variableInformation;
			this.statements = statements;
		}

		public int Add(Statement value) {
			return ((System.Collections.IList) statements).Add(value);
		}

		public override bool validate() {
			foreach (Statement statement in this) {
				if (!statement.validate()) return false;
			}

			return true;
		}

		public override string toUsableToC() {
			string output = "{";

			foreach (Statement statement in statements) {
				output += statement.toUsableToC();
			}

			output += "}";
			return output;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<Statement> GetEnumerator() {
			return statements.GetEnumerator();
		}
	}
}