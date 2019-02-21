using System;
using System.Collections.Generic;
using System.Dynamic;
using COOP.core.inheritence;

namespace COOP.core {
	public class COOPFunction {

		private string name;
		private bool hasReturnType;
		private COOPClass returnType;
		private List<COOPClass> inputTypes;
		private Dictionary<string, COOPClass> varNames;
		private AccessLevel accessLevel = 0;
		private bool isStatic;
		

		private string body;
		public bool bodyInC { get; set; } = false;

		public COOPClass owner { get; set; }

		public COOPFunction(string name, COOPClass returnType, List<COOPClass> inputTypes, Dictionary<string, COOPClass> varNames) {
			this.name = name;
			this.inputTypes = inputTypes;
			this.varNames = varNames;
			this.returnType = returnType;
			hasReturnType = true;
		}
		
		public COOPFunction(string name, COOPClass returnType, Dictionary<string, COOPClass> varNames) {
			this.name = name;
			this.inputTypes = new List<COOPClass>(varNames.Values);
			this.varNames = varNames;
			this.returnType = returnType;
			hasReturnType = true;
		}
		
		public COOPFunction(string name, List<COOPClass> inputTypes, Dictionary<string, COOPClass> varNames) {
			this.name = name;
			this.inputTypes = inputTypes;
			this.varNames = varNames;
			hasReturnType = false;
		}
		
		public COOPFunction(string name, COOPClass returnType, List<KeyValuePair<string, COOPClass>> vars = null) {
			this.name = name;
			this.returnType = returnType;
			hasReturnType = true;
			inputTypes = new List<COOPClass>();
			varNames = new Dictionary<string, COOPClass>();
			if (vars != null) {
				foreach (var keyValuePair in vars) {
					inputTypes.Add(keyValuePair.Value);
					varNames.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public COOPFunction(string name, List<KeyValuePair<string, COOPClass>> vars) {
			this.name = name;
			inputTypes = new List<COOPClass>();
			varNames = new Dictionary<string, COOPClass>();
			foreach (var keyValuePair in vars) {
				inputTypes.Add(keyValuePair.Value);
				varNames.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public string Name => name;

		public List<COOPClass> InputTypes => inputTypes;

		public bool HasReturnType => hasReturnType;

		public COOPClass ReturnType => returnType;

		public Dictionary<string, COOPClass> VarNames => varNames;

		public AccessLevel AccessLevel{
			get => accessLevel;
			set => accessLevel = value;
		}

		public bool IsStatic {
			get => isStatic;
			set => isStatic = value;
		}

		public string Body{
			get => body;
			set => body = value;
		}

		public override string ToString() {
			string output = owner.Name + "::" + name;
			output += "(";
			if (inputTypes.Count > 0) output += inputTypes[0].Name;
			for (var i = 1; i < inputTypes.Count; i++) {
				output += ", " + inputTypes[i].Name;
			}
			output += "){\n";
			output += body;
			output += "\n}";
			return output;
		}
	}
}