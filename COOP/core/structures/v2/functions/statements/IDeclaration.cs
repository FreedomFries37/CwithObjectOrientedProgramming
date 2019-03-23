using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {
	public interface IDeclaration {

		COOPType getVarType();
		string getVarName();
		
	}
}