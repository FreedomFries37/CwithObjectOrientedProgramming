using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using COOP.core.compiler.converters.ConvertedInformation;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core.compiler.converters {
	public class COOPFunctionConverter : IConverter<COOPFunction, FunctionConvertedInformation> {
		private readonly Dictionary<NameInputTypePair, string> originalNameAndInputTypesToMangledName;
		private readonly Dictionary<NameInputTypePair, bool> originalNameAndInputTypesToisStatic;
		private readonly Dictionary<string, COOPClass> functionToReturnType;
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
			functionToReturnType = new Dictionary<string,COOPClass>();
			availableClasses = hierarchy.getLineage(parentClass);
			availableClasses.AddRange(parentClass.imports);
			generateDictionaries();
		}
		

		public Collection<FunctionConvertedInformation> convert(COOPFunction coopObject, ClassHierarchy hierarchy) {
			AccessLevel accessLevel = coopObject.AccessLevel;
			bool isStatic = coopObject.IsStatic;
			string signature = generateSignature(coopObject);
			string body = coopObject.bodyInC? coopObject.Body : fixBody(coopObject.Body, generateTypeNames(coopObject), coopObject.IsStatic);
			
			FunctionConvertedInformation functionConvertedInformation = new FunctionConvertedInformation
				(accessLevel, isStatic, signature, body, coopObject.ReturnType, coopObject.InputTypes);
			functionConvertedInformation.OriginalName = coopObject.Name;
			
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

		private string fixBody(string originalBody, List<TypeName> existingInformation, bool parentFunctionIsStatic) {
			string modified = originalBody;

			
			
			Regex blocks = new Regex(@"{.*}");
			//string[] outOfBlocks = Regex.Split(modified, "\\{.*\\}");
			
			
			Dictionary<string, COOPClass> vars = new Dictionary<string, COOPClass>();
			
			if (!parentFunctionIsStatic) {
				foreach (var parentClassVarName in parentClass.VarNames.Keys) {

					vars.TryAdd(parentClassVarName, parentClass.VarNames[parentClassVarName]);
				}

			}
			
			foreach (TypeName typeName in existingInformation) {
				vars.TryAdd(typeName.name.Remove(0, 2), typeName.@class);
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

				modified = modified.Replace(type, coopClass.convertToC() + " ");
			}
			
			foreach (Match match in declareAndAssign.Matches(modified)) {
				string type = match.Groups["type"].Value;
				string name = match.Groups["name"].Value;
				COOPClass coopClass = classHierarchy.getClass(type);
				vars.Add(name, coopClass);
				modified = Regex.Replace(modified, $"\\s+{type}\\s+", coopClass.convertToC() +" ");
				
			}
			
			foreach (COOPClass availableClass in availableClasses) {
				Regex constructorCall = new Regex ($"new\\+({availableClass.Name})");
				modified = constructorCall.Replace(modified, "__init__" + availableClass.Name);
			}
			
			//Fix all input parameters to correct types
			foreach (TypeName typeName in existingInformation) {
			
				modified = $"\t{typeName.@class.convertToC()} {typeName.name.Remove(0, 2)} = ({typeName.@class.convertToC()}) {typeName.name};\n" + modified;
			}
			
			//Regex functionCall = new Regex ("(?<caller>\\.+)\\s*\\.\\s*(?<function>\\w+)\\s*\\((?<inputs>\\s*(\\w+|\"[^\"]*\"|'.')\\s*(,\\s*(\\w+|\"[^\"]*\"|'.')\\s*)*)?\\)");
			Regex functionCall = new Regex ("[a-zA-Z_]\\w*\\s*(\\(.*\\))\\s*?(\\.\\s*[a-zA-Z_]\\w*(\\(.*\\))?)+");

			FunctionCallConverter functionCallConverter = 
				new FunctionCallConverter(originalNameAndInputTypesToMangledName, 
				originalNameAndInputTypesToisStatic, 
				functionToReturnType, 
				availableClasses,
				vars);
			functionCallConverter.parentClass = parentClass;
			functionCallConverter.hierarchy = classHierarchy;
			foreach (Match match in functionCall.Matches(modified)) {
				string f = match.Value;
				string fixedStr = functionCallConverter.convert(f, classHierarchy)[0].ConvertedFunctionCall;
				modified = modified.Replace(match.Value, fixedStr);
				
			}
			/*
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
				
				string callerSymbol = match.Groups["caller"].Value;
				bool isStatic = getIsStatic(functionName, inputTypes);
				if (!isStatic) {
					
					
					if (name == "") {
						inputTypes.Insert(0, null);
						var line = classHierarchy.getLineage(vars[callerSymbol]);
						line.Reverse();
						foreach (COOPClass coopClass in line) {
							inputTypes[0] = coopClass;
							name = getMangledName(functionName, inputTypes);
							if (name != "") break;
						}
					}
				}

				string inputParams = match.Groups["inputs"].Value;
				if (!isStatic) {
					
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
			*/

			


			return modified;
		}

		private string fixFunctionCall(string coopFunctionCall, Dictionary<string, COOPClass> vars) {
			Regex functionRegex = new Regex ("(?<caller>\\.*)\\s*\\.\\s*(?<function>\\w+)\\s*\\((?<inputs>\\s*(\\w+|\"[^\"]*\"|'.')\\s*(,\\s*(\\w+|\"[^\"]*\"|'.')\\s*)*)?\\)");
			Match match = functionRegex.Match(coopFunctionCall);
			if (!match.Value.Equals(coopFunctionCall)) return "";
			string caller = match.Groups["caller"].Value;

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


			bool isStatic = getIsStatic(functionName, inputTypes);

			if (!isStatic) {
				if (functionRegex.IsMatch(caller)) {
					string func = functionRegex.Match(caller).Value;
					string fixedFunc = fixFunctionCall(func, vars);
					
				} else if (symbol.IsMatch(caller)) {
					
				} else {
					return "";
				}
			}


			return "";
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
				if(!functionToReturnType.ContainsKey(availableFunction.Name))
					functionToReturnType.Add(availableFunction.Name, availableFunction.ReturnType);
				List<COOPClass> inputTypes = new List<COOPClass>(availableFunction.InputTypes);
				if(!availableFunction.IsStatic) inputTypes.Insert(0, availableFunction.owner);
				addFunctionToMangledNameDictionary(availableFunction.Name, inputTypes, generateFunctionName(availableFunction));
				addFunctionToStaticDictionary(availableFunction.Name, inputTypes, availableFunction.IsStatic);
			}
		}
	}
}