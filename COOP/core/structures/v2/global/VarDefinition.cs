using COOP.core.structures.v2.exceptions;
using COOP.core.structures.v2.global.type;

namespace COOP.core.structures.v2.global {
	public class VarDefinition : CConvertable{

		public COOPType type { get; }
		public string name { get; }

		public VarDefinition(COOPType type, string name) {
			this.type = type;
			this.name = name;
		}

		public string toUsableToC() {
			throw new NoCConversionPossibleException();
		}
	}
}