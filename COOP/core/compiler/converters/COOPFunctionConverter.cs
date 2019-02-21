using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core.compiler.converters {
	public class COOPFunctionConverter : IConverter<COOPFunction, FunctionConvertedInformation> {
		private class InputList : List<COOPClass> {
			public InputList() { }
			public InputList(IEnumerable<COOPClass> collection) : base(collection) { }
			public InputList(int capacity) : base(capacity) { }

			protected bool Equals(InputList other) {
				return ListsEquals(this, other);
			}
			
			private static bool ListsEquals(List<COOPClass> a, List<COOPClass> b) {
				//if (List<COOPClass>.Equals(a, b)) return true;
				if (a.Count != b.Count) return false;
				for (var i = 0; i < a.Count; i++) {
					if (!a[i].Equals(b[i])) return false;
				}

				return true;
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((InputList) obj);
			}

			public override int GetHashCode() {
				int output = 0;
				foreach (COOPClass coopClass in this) {
					output += coopClass.Name.GetHashCode() * 397;
				}

				return output;
			}
			
			
		}
		
		private class NameInputTypePair {
			public string name { get; }
			public InputList inputs { get; }

			public NameInputTypePair(string name, List<COOPClass> inputs) {
				this.name = name;
				this.inputs = new InputList(inputs);
			}

			protected bool Equals(NameInputTypePair other) {
				return string.Equals(name, other.name) && Equals(inputs, other.inputs);
			}

			

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((NameInputTypePair) obj);
			}

			public override int GetHashCode() {
				unchecked {
					return ((name != null ? name.GetHashCode() : 0) * 397) ^ (inputs != null ? inputs.GetHashCode() : 0);
				}
			}

			private sealed class NameInputsEqualityComparer : IEqualityComparer<NameInputTypePair> {
				public bool Equals(NameInputTypePair x, NameInputTypePair y) {
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;
					return string.Equals(x.name, y.name) && x.inputs.Equals(y.inputs);
				}

				public int GetHashCode(NameInputTypePair obj) {
					unchecked {
						return ((obj.name != null ? obj.name.GetHashCode() : 0) * 397) ^ (obj.inputs != null ? obj.inputs.GetHashCode() : 0);
					}
				}
			}

			public static IEqualityComparer<NameInputTypePair> nameInputsComparer { get; } = new NameInputsEqualityComparer();
		}
		private readonly Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName;
		private readonly Dictionary<NameInputTypePair, bool> originalNameAndInputTypesToisStatic;
		private readonly List<COOPClass> availableClasses;
		private COOPClass parentClass;

		public ClassHierarchy classHierarchy { get; private set; }
		

		private class TypeName {
			public COOPClass @class{ get; set; }
			public string name { get; set; }

			public TypeName(COOPClass @class, string name) {
				this.@class = @class;
				this.name = name;
			}
		}

		public COOPFunctionConverter(ClassHierarchy hierarchy, COOPClass parentClass) {
			classHierarchy = hierarchy;
			this.parentClass = parentClass;
			originalNameAndInputTypesToMangledName = new Dictionary<NameInputTypePair, string>(NameInputTypePair.nameInputsComparer);
			originalNameAndInputTypesToisStatic = new Dictionary<NameInputTypePair, bool>();
			availableClasses = hierarchy.getLineage(parentClass);
			availableClasses.AddRange(parentClass.imports);
			generateDictionaries();
		}
		

		public Collection<FunctionConvertedInformation> convert(COOPFunction coopObject, ClassHierarchy hierarchy) {
			AccessLevel accessLevel = coopObject.AccessLevel;
			bool isStatic = coopObject.IsStatic;
			string signature = generateSignature(coopObject);
			string body = coopObject.bodyInC? coopObject.Body : fixBody(coopObject.Body, generateTypeNames(coopObject));
			
			FunctionConvertedInformation functionConvertedInformation = new FunctionConvertedInformation
				(accessLevel, isStatic, signature, body, coopObject.ReturnType, coopObject.InputTypes);
			
			return new Collection<FunctionConvertedInformation> {functionConvertedInformation};
		}
		
		public Collection<FunctionConvertedInformation> convert(COOPFunction coopObject) {
			return convert(coopObject, classHierarchy);
		}

		private string generateSignature(COOPFunction function) {
			string returnType = function.ReturnType.convertToC();
			var generatedName = COOPFunctionConverter.generateFunctionName(function);

			var typeNames = generateTypeNames(function);
			string parameters = "(";
			for (var i = 0; i < typeNames.Count - 1; i++) {
				parameters += "" + typeNames[i].@class.convertToC() + " " + typeNames[i].name + ", ";
			}

			if (typeNames.Count > 0) {
				int i = typeNames.Count - 1;
				parameters += "" + typeNames[i].@class.convertToC() + " " + typeNames[i].name;
			}
			parameters += ")";

			return $"{returnType} {generatedName}{parameters}";
		}

		private static string generateFunctionName(COOPFunction function) {
			List<string> inputTypes = new List<string>(from s in function.InputTypes select s.Name);
			if (!function.IsStatic) inputTypes.Insert(0, function.owner.Name);
			string generatedName = function.Name;
			foreach (string inputType in inputTypes) {
				generatedName += "_" + inputType;
			}

			generatedName = Regex.Replace(generatedName, "\\s+", "");
			return generatedName;
		}

		private static List<TypeName> generateTypeNames(COOPFunction function) {
			List<TypeName> typeNames = new List<TypeName>();
			if (!function.IsStatic) typeNames.Add(new TypeName(function.owner, "__this"));
			typeNames.AddRange(from pair in function.VarNames select new TypeName(pair.Value, "__" + pair.Key));
			
			return typeNames;
		}

		private void addFunctionToMangledNameDictionary(string originalName, List<COOPClass> inputTypes,
			string mangledName) {
			NameInputTypePair nameInputTypePair = new NameInputTypePair(originalName, inputTypes);
			if(!originalNameAndInputTypesToMangledName.ContainsKey(nameInputTypePair)) originalNameAndInputTypesToMangledName.Add(nameInputTypePair, mangledName);
		}
		
		private string getMangledName(string originalName, List<COOPClass> inputTypes) {
			if(!originalNameAndInputTypesToMangledName.TryGetValue(
				new NameInputTypePair(originalName, inputTypes), out string newName)
			) return "";
			return newName;
		}
		
		private void addFunctionToStaticDictionary(string originalName, List<COOPClass> inputTypes,
			bool isStatic) {
			NameInputTypePair nameInputTypePair = new NameInputTypePair(originalName, inputTypes);
			if(!originalNameAndInputTypesToisStatic.ContainsKey(nameInputTypePair))
				originalNameAndInputTypesToisStatic.Add(new NameInputTypePair(originalName, inputTypes), isStatic);
		}
		
		private bool getIsStatic(string originalName, List<COOPClass> inputTypes) {
			if(!originalNameAndInputTypesToisStatic.TryGetValue(
				new NameInputTypePair(originalName, inputTypes), out bool value)
			) return false;
			return value;
		}

		private string fixBody(string originalBody, List<TypeName> existingInformation) {
			string modified = originalBody;
			Regex blocks = new Regex(@"{.*}");
			//string[] outOfBlocks = Regex.Split(modified, "\\{.*\\}");
			
			
			Dictionary<string, COOPClass> vars = new Dictionary<string, COOPClass>();
			foreach (TypeName typeName in existingInformation) {
				vars.Add(typeName.name, typeName.@class);
			}
			
			// Fix Variables
			Regex declareOnly = new Regex(@"(?<type>\w+)(\s+(?<names>\w+(,\s*\w+\s*)*));");
			Regex declareAndAssign = new Regex(@"(?<type>\w+)\s+(?<name>\w+)\s*=\s*[^;]+;");
			foreach (Match match in declareOnly.Matches(modified)) {
				string type = match.Groups["type"].Value;
				if(ReservedWords.isReserved(type)) continue;
				string[] decs = Regex.Split(match.Groups["names"].Value, "\\s*,\\s*");
				COOPClass coopClass = classHierarchy.getClass(type);
				foreach (string dec in decs) {
					if(ReservedWords.isReserved(dec)) continue;
					vars.Add(dec, coopClass);
				}

				modified = modified.Replace(type, coopClass.convertToC());
			}
			
			foreach (Match match in declareAndAssign.Matches(modified)) {
				string type = match.Groups["type"].Value;
				string name = match.Groups["name"].Value;
				COOPClass coopClass = classHierarchy.getClass(type);
				vars.Add(name, coopClass);
				modified = modified.Replace(type, coopClass.convertToC());
			}
			
			foreach (COOPClass availableClass in availableClasses) {
				Regex constructorCall = new Regex ($"new\\+({availableClass.Name})");
				modified = constructorCall.Replace(modified, "__init__" + availableClass.Name);
			}
			
			//Fix all input parameters to correct types
			foreach (TypeName typeName in existingInformation) {
				modified = $"\t{typeName.@class.convertToC()} {typeName.name.Remove(0, 2)} = ({typeName.@class.convertToC()}) {typeName.name};\n" + modified;
			}
			
			Regex functionCall = new Regex ("(?<caller>\\w+)\\s*\\.\\s*(?<function>\\w+)\\s*\\((?<inputs>\\s*(\\w+|\"[^\"]*\"|'.')\\s*(,\\s*(\\w+|\"[^\"]*\"|'.')\\s*)*)?\\)");
			foreach (Match match in functionCall.Matches(modified)) {
				string functionName = match.Groups["function"].Value;
				string inputsFull = match.Groups["inputs"].Value;
				string[] inputs = Regex.Split(inputsFull, "\\s*,\\s*");
				
				Regex symbol = new Regex("\\w+"), character = new Regex("'.'"), @string = new Regex("\".*\"");
				Regex integer = new Regex("\\d+.?");
				Regex floatingPoint = new Regex("\\d+\\.\\d+");
				List<COOPClass> inputTypes = new List<COOPClass>();
				foreach (string input in from s in inputs select "__" + s) {
					
					if (@string.IsMatch(input)) {
						inputTypes.Add(COOPClass.String);
					}else if (character.IsMatch(input)) {
						inputTypes.Add(COOPPrimitives.@byte);
					}else if (integer.IsMatch(input)) {
						inputTypes.Add(COOPPrimitives.integer);
					}else if (floatingPoint.IsMatch(input)) {
						inputTypes.Add(COOPPrimitives.@float);
					}else if (symbol.IsMatch(input)) {
						
						if(vars.TryGetValue(input, out COOPClass type)){
							inputTypes.Add(type);
						}
					}
				}

				string name = getMangledName(functionName, inputTypes);
				bool isStatic = getIsStatic(functionName, inputTypes);
				if (name == "") {
					inputTypes.Insert(0, null);
					var line = classHierarchy.getLineage(parentClass);
					line.Reverse();
					foreach (COOPClass coopClass in line) {
						inputTypes[0] = coopClass;
						name = getMangledName(functionName, inputTypes);
						if (name != "") break;
					}
					
					
				}

				string inputParams = match.Groups["inputs"].Value;
				if (!isStatic) {
					string callerSymbol = match.Groups["caller"].Value;
					if(!(from f in existingInformation select f.name).Contains(callerSymbol) &&
					   !vars.ContainsKey(callerSymbol)) {
						callerSymbol = "this->" + callerSymbol;
					}
					inputParams = callerSymbol + ", " + inputParams;
				}
				
				string modifiedCall = name + $"({inputParams})";
				int index = match.Index;
				string temp = modified.Substring(0, index);
				temp += modifiedCall;
				temp += modified.Substring(index + match.Value.Length);
				modified = temp;
			}


			


			return modified;
		}

	

		private List<string> getAvailableMangledNames(COOPClass coopClass) {
			List<COOPFunction> functions = getAvailableFunctions(coopClass, classHierarchy);
			List<string> output = new  List<string>();
			output.AddRange(from f in functions select getMangledName(f.Name, f.InputTypes));

			return output;
		}

		private List<COOPFunction> getAvailableFunctions(COOPClass coopClass, ClassHierarchy hierarchy) {
			List<COOPFunction> output = new List<COOPFunction>(coopClass.Functions.Values);
			
			List<COOPClass> lineageClasses = new List<COOPClass>(hierarchy.getLineage(coopClass));
			lineageClasses.Remove(coopClass);
			
			foreach (COOPClass lineageClass in lineageClasses) {
				output.AddRange(from f in lineageClass.Functions.Values where (int) f.AccessLevel >= 1 select f);
			}
			foreach (COOPClass coopClassImport in coopClass.imports) {
				output.AddRange(from f in coopClassImport.Functions.Values where f.AccessLevel == AccessLevel.Public select f);
			}

			return output;
		}

		private void generateDictionaries() {
			List<COOPFunction> availableFunctions = getAvailableFunctions(parentClass, classHierarchy);
			foreach (COOPFunction availableFunction in availableFunctions) {
				List<COOPClass> inputTypes = new List<COOPClass>(availableFunction.InputTypes);
				if(!availableFunction.IsStatic) inputTypes.Insert(0, availableFunction.owner);
				addFunctionToMangledNameDictionary(availableFunction.Name, inputTypes, generateFunctionName(availableFunction));
				addFunctionToStaticDictionary(availableFunction.Name, inputTypes, availableFunction.IsStatic);
			}
		}
	}
}