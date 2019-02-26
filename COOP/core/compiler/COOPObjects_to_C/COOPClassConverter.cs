using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using COOP.core.compiler.converters.ConvertedInformation;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core.compiler.converters {
	public class COOPClassConverter : IConverter<COOPClass, FileConvertedInformation> {

		public bool humanReadable { get; set; } = true;

		public Collection<FileConvertedInformation> convert(COOPClass coopObject, ClassHierarchy hierarchy) {
			var output = new Collection<FileConvertedInformation>();
			COOPClass parent = hierarchy.getParent(coopObject);
			bool createProtected, createPrivate;
			string publicStructure = generatePublicStructure(coopObject, parent, out createProtected, out createPrivate);

			string cFile = "", 
				protectedHeader = $"#ifndef SRC_PROTECTED_FILE_{coopObject.Name}\n#define SRC_PROTECTED_FILE_{coopObject.Name}\n", 
				publicHeader = $"#ifndef SRC_PUBLIC_FILE_{coopObject.Name}\n#define SRC_PUBLIC_FILE_{coopObject.Name}\n";
	
			foreach (COOPClass coopObjectImport in coopObject.imports) {
				cFile += $"#include \"{coopObjectImport.Name}.h\"\n";
			}

			if (coopObject.Parent != null) {
				cFile += $"#include \"{coopObject.Parent.Name}_protected.h\"\n";
			}

			cFile += $"#include \"{coopObject.Name}_protected.h\"\n";
			
			
			if (createPrivate) cFile += generatePrivateStructure(coopObject);


			protectedHeader +=  $"#include \"{coopObject.Name}.h\"\n";
			if (createProtected) {
				protectedHeader += generateProtectedStructure(coopObject);
			}
			
			publicHeader += publicStructure;
			
			
			COOPFunctionConverter functionConverter = new COOPFunctionConverter(hierarchy, coopObject);
			List<FunctionConvertedInformation> functionConvertedInformations = new List<FunctionConvertedInformation>();
			foreach (var coopObjectFunction in coopObject.getFunctions()) {
				var f = functionConverter.convert(coopObjectFunction);
				functionConvertedInformations.AddRange(f);
			}
			
			foreach (FunctionConvertedInformation functionConvertedInformation in 
				from f in functionConvertedInformations where f.accessLevel == AccessLevel.Private select f) {
				cFile += functionConvertedInformation.signature + ";\n";
			}
			
			foreach (FunctionConvertedInformation functionConvertedInformation in 
				from f in functionConvertedInformations where f.accessLevel == AccessLevel.Protected select f) {
				protectedHeader += functionConvertedInformation.signature + ";\n";
			}

			bool hasMain = false;
			string sig = "";
			foreach (FunctionConvertedInformation functionConvertedInformation in 
				from f in functionConvertedInformations where f.accessLevel == AccessLevel.Public select f) {
				publicHeader += functionConvertedInformation.signature + ";\n";
				//Console.WriteLine(functionConvertedInformation.signature);
				if (functionConvertedInformation.OriginalName == "main") {
					Console.WriteLine("Main Method Found: " + functionConvertedInformation.signature);
					hasMain = true;
					sig = functionConvertedInformation.signature;
				}
			}
			
			foreach (FunctionConvertedInformation functionConvertedInformation in functionConvertedInformations) {
				cFile += functionConvertedInformation.signature + "{\n";
				cFile += functionConvertedInformation.body;
				cFile += "}\n";
			}

			publicHeader += "#endif";
			protectedHeader += "#endif";
			
			if (humanReadable) {
				cFile = toHumanReadable(cFile);
				publicHeader = toHumanReadable(publicHeader);
				protectedHeader = toHumanReadable(protectedHeader);
			}
			
			output.Add(new FileConvertedInformation(coopObject.Name + ".c", cFile));
			output.Add(new FileConvertedInformation(coopObject.Name + "_protected.h", protectedHeader));
			output.Add(new FileConvertedInformation(coopObject.Name + ".h", publicHeader));

			if (hasMain) {
				output[2].hasMainMethod = true;
				output[2].mainMethod = sig;
				Console.WriteLine("Main Method Confirmed: " + sig);
			}
			
			return output;
		}

		private string generatePublicStructure(COOPClass coopClass, COOPClass parent, out bool createProtected, out bool createPrivate) {

			string publicStruct = "";
			string privateStruct = "";
			string protectedStruct = "";

			createPrivate = false;
			createProtected = false;
			
			List<string> publicVars = new List<string>();
			
			
			foreach (var coopClassVarName in coopClass.VarNames) {
				string name = coopClassVarName.Key;
				string className = coopClassVarName.Value.Name;

				

				if (coopClass.getAccessLevel(name).Equals(AccessLevel.Public)) {
					COOPClass varClass = coopClass.VarNames[name];
					string entry = $"{varClass.convertToC()} {name};";
					publicVars.Add(entry);
				}else if (privateStruct.Equals("") && coopClass.getAccessLevel(name).Equals(AccessLevel.Private)) {
					privateStruct = $"struct {coopClass.Name}_private_struct private;";
					createPrivate = true;

				}else if (protectedStruct.Equals("") && coopClass.getAccessLevel(name).Equals(AccessLevel.Protected)) {
					protectedStruct = $"struct {coopClass.Name}_protected_struct protected;";
					createProtected = true;
				}
			}

			if (publicVars.Count > 0) {
				publicStruct += $"struct {coopClass.Name}_public_struct{{";
				foreach (string publicVar in publicVars) {
					publicStruct += publicVar;
				}

				publicStruct += "} public;";
			}

			string parentStr = parent == null? "" : $"struct {parent.Name} parent;";

			return $"struct {coopClass.Name} {{ " +
						parentStr +
						$"{publicStruct}" +
						$"{protectedStruct}" +
						$"{privateStruct}" +
					"};";
		}

		private string generatePrivateStructure(COOPClass coopClass) {


			return "";
		}
		
		private string generateProtectedStructure(COOPClass coopClass) {


			return "";
		}

		private string toHumanReadable(string s) {
			string modified = s, output = "";
			modified = modified.Replace("}", "\n}\n");
			modified = Regex.Replace(modified, "\\{", "{\n" );
			modified = Regex.Replace(modified, ";", ";\n");
			string[] seperated = Regex.Split(modified, "\\n+");


			int indentLevel = 0;
			
			foreach (string line in seperated) {
				if (line != "") {
					Regex tabs = new Regex("\t*(?<statement>.*)");
					string lineWithIndent = "";
					if (line.Contains("}") && indentLevel > 0) {
						indentLevel--;
					}
					for (int i = 0; i < indentLevel; i++) {
						lineWithIndent += "\t";
					}

					lineWithIndent += tabs.Match(line).Groups["statement"].Value + "\n";

					if (line.Contains("{")) {
						indentLevel++;
					}

					

					output += lineWithIndent;
				}
			}
			
			
			return output;
		}
	}
}