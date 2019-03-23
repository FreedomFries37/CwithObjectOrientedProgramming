using System;
using System.Collections.Generic;
using COOP.core.structures.v2.exceptions;
using COOP.core.structures.v2.functions.statements;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.function_bodies {
	public class VariableInformation {

		public Ownership<Body> ownership { get; }

		private Dictionary<string, COOPType> types;
		private Dictionary<string, bool> declarationCreated;

		private bool varIsConnectedToClass(string s) => types[s].isStrictlyClass();

		private Queue<string> createdCStatements;

		public VariableInformation(Ownership<Body> owner) {
			ownership = owner;
			types = new Dictionary<string, COOPType>();
			foreach (VarDefinition ownerParameter in owner.owner.parameters) {
				types.Add(ownerParameter.name, ownerParameter.type);
			}
			declarationCreated = new Dictionary<string, bool>();
			createdCStatements = new Queue<string>();
		}

		public VariableInformation(VariableInformation information) {
			
			ownership = information.ownership;
			types = new Dictionary<string, COOPType>(information.types);
			declarationCreated = new Dictionary<string, bool>(information.declarationCreated);
			createdCStatements = new Queue<string>(information.createdCStatements);
		}

		public void processDeclaration(IDeclaration declaration) {
			processDeclaration(declaration, out bool cGenerated, out string str);
			if(cGenerated) createdCStatements.Enqueue(str);
		}
		
		
		public void processDeclaration(IDeclaration declaration, out bool cGenerated, out string str) {
			string varName = declaration.getVarName();
			if(types.ContainsKey(varName)) throw new VariableAlreadyDeclaredException(varName);
			COOPType coopType = declaration.getVarType();
			types.Add(varName, coopType);

			cGenerated = false;
			str = "";
			if (coopType.isStrictlyClass()) {
				cGenerated = true;
				str = $"{(coopType as COOPClass)?.toUsableToC()} {varName}";
			}
		}
		public void processAssignment(IAssignment assignment, out bool cGenerated, out string str) {
			string varName = assignment.getVarName();
			if(!types.ContainsKey(varName)) throw new VariableDoesNotExistException(varName);
			COOPType coopType = assignment.getCOOPObject().actualType, varType = types[varName];
			

			cGenerated = false;
			str = "";
			if (coopType.Equals(varType)) {
				cGenerated = true;
				str = $"{varName} = {assignment.getCOOPObjectString()}";
			}else if (varType.isParent(coopType) && !varType.isStrictlyClass() && coopType.isStrictlyClass()) {
				types[varName] = coopType;
				cGenerated = true;
				str = $"{(coopType as COOPClass).toUsableToC()} {varName} = {assignment.getCOOPObjectString()}";
			}
		}

		public void processAssignment(IAssignment assignment) {
			processAssignment(assignment, out bool generated, out string str);
			if(generated) createdCStatements.Enqueue(str);
		}
	}
}