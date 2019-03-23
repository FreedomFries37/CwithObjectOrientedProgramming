using System;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type;
using COOP.core.structures.v2.global.type.hierarchy;

namespace COOP.core.compiler.COOP_objects_to_C_file {
	public class COOPClassFileCreator: FileCreator {

		private AdvancedTypeHierarchy hierarchy;
		private COOPClass @class;
		private bool hasParent => @class.parent != null;
		
		

		public COOPClassFileCreator(AdvancedTypeHierarchy hierarchy, COOPClass @class) {
			this.hierarchy = hierarchy;
			this.@class = @class;
		}

		
		/*
		 * 1. public header
		 * 2. protected header
		 * 3. directory header
		 * 4. private header
		 * 5. sealed header
		 * 6. c file
		 */
		public int createFiles(string outputPath) {
			int filesCreated = 1;
			
			if (headerNecessary(AccessLevel.Public)) {
				filesCreated++;
			}
			if (headerNecessary(AccessLevel.Directory)) {
				filesCreated++;
			}
			if (headerNecessary(AccessLevel.Protected)) {
				filesCreated++;
			}
			if (headerNecessary(AccessLevel.Private)) {
				filesCreated++;
			}
			if (headerNecessary(AccessLevel.Sealed)) {
				filesCreated++;
			}

			return filesCreated;
		}

		private bool headerNecessary(AccessLevel level) {
			if (@class.getFields(level, AccessLevelRule.@equals).Length > 0) return true;
			
			

			return true;
		}

		private string publicHeader() {
			string output = "";
			foreach (string s in generateInitalPreprocessorCommands()) {
				output += s + "\n";
			}

			output += publicStruct();

			foreach (var s in generateFinalPreprocessorCommands()) {
				output += s + "\n";
			}
			return output;
		}

		private string[] generateInitalPreprocessorCommands() {
			
			return new string[0];
		}
		
		private string[] generateFinalPreprocessorCommands() {
			
			return new string[0];
		}

		public string createFunctionReferenceMap() {

			return "";
		}

		private string newFunction() {
			string output = $"{@class.toUsableToC()}* __new__{@class.Name}(){{\n";

			output += $"{@class.toUsableToC()}* output = malloc(sizeof({@class.toUsableToC()})";
			if (hasParent) {
				output += $"output->super = __new__{@class.parent.Name}();\n";
			}

			output += "type_info info;";
			output += createTypeInfo("info");
			output += "output->t_info = info;\n";

			

			output += "return output;\n";
			return output + "}\n";
		}

		private string createTypeInfo(string varName) {
			return $"{varName}.name = {@class.Name};\n";
		}
		
	

		private string publicStruct() {
			Field[] publicFields = @class.getFields(AccessLevel.Public, AccessLevelRule.equals);
			COOPClass parentClass = hierarchy.findNextAvailableParentClass(@class);
			string parentClassString = hasParent? "" : parentClass.toUsableToC() + " super;\n";

			string publicFieldsString = "";
			foreach (Field publicField in publicFields) {
				string type = hierarchy.findNextAvailableParentClass((COOPClass) publicField.type).toUsableToC();
				publicFieldsString += $"{type} {publicField.name};\n";
			}

			return $@"
{@class.toUsableToC()}{{
	{parentClassString}
	struct type_info t_info;
	struct function_references *functions;
	struct {@class.Name}_external_references external;
	struct {@class.Name}_protected protected;
	struct {@class.Name}_private private;
	{publicFieldsString}
}};";
		}
	}
}