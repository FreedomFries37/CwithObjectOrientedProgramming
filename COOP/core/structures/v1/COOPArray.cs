using System.Collections.Generic;
using System.Collections.ObjectModel;
using COOP.core.structures.v2.global.modifiers;

namespace COOP.core.structures.v1 {
	public class COOPArray : COOPClass{

		private COOPClass baseClass;
		private uint length;

		public COOPArray(COOPClass baseClass, uint length) : base(baseClass.Name + "_array", Base, new Collection<COOPFunction> {bracketOperator(baseClass), getFunction(baseClass)}, new Dictionary<string, COOPClass>()) {
			this.baseClass = baseClass;
			this.length = length;
			Functions["operator []"].AccessLevel = AccessLevel.Public;
			Functions["get"].AccessLevel = AccessLevel.Public;
		}


		private static COOPFunction bracketOperator(COOPClass baseClass) {
			return new COOPFunction("operator []", baseClass, new List<COOPClass>{ COOPPrimitives.uinteger}, new Dictionary<string, COOPClass>()); 
		}
		
		private static COOPFunction getFunction(COOPClass baseClass) {
			return new COOPFunction("get", baseClass, new Dictionary<string, COOPClass> {{"index", COOPPrimitives.integer}}); 
		}

		public override string convertToC() {
			return baseClass.convertToC() + $"[{length}]";
		}
	}
}