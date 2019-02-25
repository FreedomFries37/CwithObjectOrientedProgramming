using System.Collections.Generic;
using System.Collections.ObjectModel;
using COOP.core.inheritence;

namespace COOP.core.structures {
	public class PrimitiveCOOPClass : COOPClass {
		public PrimitiveCOOPClass(string name) : base(name, Base, new Collection<COOPFunction>(),
			new Dictionary<string, COOPClass>()) {
			genFile = false;
		}

		public override string convertToC() {
			return Name;
		}
	}
}