using System.Collections.Generic;
using COOP.core.inheritence;

namespace COOP.core.compiler {
	public class FunctionConvertedInformation : ConvertedInformation{

		public AccessLevel accessLevel { get; }
		public bool isStatic { get; }
		public string signature { get; }
		public string body { get; set; }

		public COOPClass returnType { get; }

		public List<COOPClass> inputTypes { get; }

		public string OriginalName { get; set; }
		public string MangledName { get; set; }


		public FunctionConvertedInformation(AccessLevel accessLevel, bool isStatic, string signature, string body, COOPClass returnType, List<COOPClass> inputTypes) {
			this.accessLevel = accessLevel;
			this.isStatic = isStatic;
			this.signature = signature;
			this.body = body;
			this.returnType = returnType;
			this.inputTypes = inputTypes;
		}

		public FunctionConvertedInformation(AccessLevel accessLevel, bool isStatic, string signature, COOPClass returnType, List<COOPClass> inputTypes) {
			this.accessLevel = accessLevel;
			this.isStatic = isStatic;
			this.signature = signature;
			this.returnType = returnType;
			this.inputTypes = inputTypes;
		}

		public override string ToString() {
			return $"{signature}{{\n{body}\n}}";
		}
	}
}