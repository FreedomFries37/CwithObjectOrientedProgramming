using System.Collections.Generic;
using COOP.core.structures.v2.functions.statements;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.function_bodies {
	public class BodyInstance {

		private Body b;
		public VariableInformation variableInformation { get; }

		public BodyInstance(Body b, List<COOPObject> objects) {
			this.b = b;
			variableInformation = new VariableInformation(Ownership<Body>.ownership(b));
			int index = 0;
			foreach (VarDefinition varDefinition in b.parameters.correctOrder) {
				variableInformation.processDeclaration(new InstanceParameterDeclaration(varDefinition.type, varDefinition.name));
				variableInformation.processAssignment(new InstanceParameterAssignment(varDefinition.name, objects[index++]));
				
			}
			
			
		}

		private class InstanceParameterDeclaration : IDeclaration {

			private COOPType type;
			private string name;

			public InstanceParameterDeclaration(COOPType type, string name) {
				this.type = type;
				this.name = name;
			}


			public COOPType getVarType() {
				return type;
			}

			public string getVarName() {
				return name;
			}
		}
		
		private class InstanceParameterAssignment : IAssignment {

			
			private string   name;
			private COOPObject o;

			public InstanceParameterAssignment(string name, COOPObject o) {
				this.o = o;
				this.name = name;
			}


			public COOPObject getCOOPObject() {
				return o;
			}

			public string getVarName() {
				return name;
			}

			public string getCOOPObjectString() {
				throw new System.NotImplementedException();
			}
		}
	}
}