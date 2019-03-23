using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using COOP.core.structures.v2.functions.statements;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.function_bodies {
	public class Body : CConvertable{

		public Ownership<COOPFunction> ownership { get;  }
		public Modifiers modifiers { get; }
		public Parameters parameters { get; }
		private Block block;


		public Body(List<Statement> statements, Ownership<COOPFunction> ownership, Modifiers modifiers, Parameters parameters) {
			block = new Block(ownership, new VariableInformation(Ownership<Body>.ownership(this)), statements);
			this.ownership = ownership;
			this.modifiers = modifiers;
			this.parameters = parameters;
		}

		public Body(Block block, Ownership<COOPFunction> ownership, Modifiers modifiers, Parameters parameters) {
			this.block = block;
			this.ownership = ownership;
			this.modifiers = modifiers;
			this.parameters = parameters;
		}

		public string toUsableToC() {
			throw new System.NotImplementedException();
		}

		public InputList inputList() => new InputList(parameters.types);
		

		public FunctionCall toFunctionCall() {
			return new FunctionCall(ownership.owner, this);
		}

		public bool validate() {
			List<VarDeclaration> declarations;



			return true;
		}

		public bool couldExecuteOn(List<COOPObject> objects) {
			
			for (var i = 0; i < objects.Count; i++) {
				COOPType coopType = parameters[i].type;
				if (!coopType.isParent(objects[i].actualType)) return false;
				
			}

			return true;
		}
		
		public bool couldDirectlyExecuteOn(List<COOPObject> objects) {
			bool allClasses = true;
			for (var i = 0; i < objects.Count; i++) {
				COOPType coopType = parameters[i].type;
				if (!coopType.isParent(objects[i].actualType)) return false;
				if (!coopType.isStrictlyClass()) allClasses = false;
			}

			return allClasses;
		}

		public Body fixBody(params COOPClass[] classes) => fixBody(classes.ToList());
		public Body fixBody(params COOPObject[] classes) => fixBody(classes.ToList());
		public Body fixBody(List<COOPObject> classes) => fixBody((from f in classes select f.actualType).ToList());
		
		public Body fixBody(List<COOPClass> classes) {
			int paramIndex = 0;
			List<VarDefinition> definitions = new List<VarDefinition>();
			for (var i = 0; i < classes.Count; i++) {
				while (parameters[paramIndex].type.isStrictlyClass()) {
					definitions.Add(parameters[paramIndex]);
					paramIndex++;
				}

				if (parameters[paramIndex].type.isParent(classes[i])) {
					definitions.Add(new VarDefinition(classes[i], parameters[paramIndex].name));
				}
			}
			Parameters p = new Parameters(definitions);
			Body output = new Body(block, ownership, modifiers, p);


			return output;
		}

		
	}
}