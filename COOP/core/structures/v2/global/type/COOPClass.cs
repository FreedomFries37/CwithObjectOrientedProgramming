using System.Collections.Generic;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.global.type.included;

namespace COOP.core.structures.v2.global.type {
	public class COOPClass : COOPAbstract, CConvertable {
		

		public List<COOPFunction> functions { get; }
		


		public COOPClass(string name, COOPClass super) : base(name, super) {
			
			functions = new List<COOPFunction>();
		}
		
		public COOPClass(string name) : this(name, IncludedClasses.Object) { }

		public string toUsableToC() {
			throw new System.NotImplementedException();
		}

		public override bool isStrictlyInterface() => false;

		public override bool isStrictlyAbstract() => false;

		public override bool isStrictlyClass() => true;
	}
}