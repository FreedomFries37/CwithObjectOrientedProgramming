using COOP.core.structures.v2.functions;

namespace COOP.core.structures.v2.global.type.included {
	public class IncludedClasses {

		public static COOPClass Object { get; }
		public static COOPClass String { get; }








		static IncludedClasses (){
			Object = new COOPClass("Object", null);
			String = new COOPClass("String");
			
			COOPFunction ToString = new COOPFunction(Ownership<COOPClass>.ownership(Object), String, "ToString");
			
		}
	}
}