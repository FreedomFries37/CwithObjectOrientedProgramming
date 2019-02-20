using System.Collections.Generic;
using System.Collections.ObjectModel;
using COOP.core.inheritence;


namespace COOP.core.structures {
	public class COOPArray : COOPClass, NonGenericCConversion{

		private COOPClass baseClass;
		private uint length;

		public COOPArray(COOPClass baseClass, uint length) : base(baseClass.Name + "_array", Base, new Collection<COOPFunction> {bracketOperator(baseClass), getFunction(baseClass)}, new Dictionary<string, COOPClass>()) {
			this.baseClass = baseClass;
			this.length = length;
			Functions["get"].AccessLevel = AccessLevel.Public;
		}


		private static COOPFunction bracketOperator(COOPClass baseClass) {
			return new COOPFunction("operator []", baseClass, new List<COOPClass>{ COOPPrimitives.uinteger}, new Dictionary<string, COOPClass>()); 
		}
		
		private static COOPFunction getFunction(COOPClass baseClass) {
			return new COOPFunction("get", baseClass, new Dictionary<string, COOPClass> {{"index", COOPPrimitives.integer}}); 
		}

		public string convertToCNonGeneric() {
			string output = "";
			if (baseClass is NonGenericCConversion) {
				output += ((NonGenericCConversion) baseClass).convertToCNonGeneric();
			} else {
				output += "struct " + baseClass.Name + "*";
			}

			output += $"[{length}]";
			return output;
		}
	}
}