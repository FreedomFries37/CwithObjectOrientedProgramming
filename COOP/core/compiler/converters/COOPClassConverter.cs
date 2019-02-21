using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core.compiler.converters {
	public class COOPClassConverter : IConverter<COOPClass, FileConvertedInformation> {
		
		
		
		public Collection<FileConvertedInformation> convert(COOPClass coopObject, ClassHierarchy hierarchy) {
			var output = new Collection<FileConvertedInformation>();
			COOPClass parent = hierarchy.getParent(coopObject);
			bool createProtected, createPrivate;
			string publicStructure = generatePublicStructure(coopObject, parent, out createProtected, out createPrivate);

			string cFile;
			if (createPrivate) cFile = generatePrivateStructure(coopObject);
			else {
				cFile = "";
			}
			COOPFunctionConverter functionConverter = new COOPFunctionConverter(hierarchy, coopObject);
			List<FunctionConvertedInformation> functionConvertedInformations = new List<FunctionConvertedInformation>();
			foreach (var coopObjectFunction in coopObject.Functions.Values) {
				var f = functionConverter.convert(coopObjectFunction);
				functionConvertedInformations.AddRange(f);
			}
			
			foreach (FunctionConvertedInformation functionConvertedInformation in functionConvertedInformations) {
				cFile += functionConvertedInformation.signature + ";\n";
			}
			
			foreach (FunctionConvertedInformation functionConvertedInformation in functionConvertedInformations) {
				cFile += functionConvertedInformation.signature + "{\n";
				cFile += functionConvertedInformation.body;
				cFile += "}\n";
			}
			
			Console.WriteLine(publicStructure);
			Console.WriteLine(cFile);

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
					"}};";
		}

		private string generatePrivateStructure(COOPClass coopClass) {


			return "";
		}
	}
}