using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {
	public interface IAssignment {

		string getVarName();
		COOPObject getCOOPObject();

		string getCOOPObjectString();
	}
}