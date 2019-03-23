using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using COOP.core.structures.v2.functions.function_bodies;
using COOP.core.structures.v2;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type;
using global::COOP.core.structures.v2.global;


namespace COOP.core.structures.v2.functions {
	public class COOPFunction {

		public Ownership<COOPClass> ownership { get; }
		public COOPClass returnType { get; }
		public string name { get; }

		private Dictionary<InputList, Body> inputsListToBody;
		private Collection<Body> bodies;

		public COOPFunction(Ownership<COOPClass> ownership, COOPClass returnType, string name) {
			this.ownership = ownership;
			this.returnType = returnType;
			this.name = name;
			bodies = new Collection<Body>();
			inputsListToBody = new Dictionary<InputList, Body>();
		}

		public bool Add(Body value) {
			InputList inputList = new InputList();
			foreach (COOPType parametersType in value.parameters.types) {
				inputList.Add(parametersType);
			}

			if (inputsListToBody.ContainsKey(inputList)) return false;

			inputsListToBody.Add(inputList, value);
			bodies.Add(value);
			return true;

		}

		public Body this[InputList list] {
			get {
				if (!inputsListToBody.TryGetValue(list, out Body body)) return null;
				return body;
			}
		}

		public List<FunctionCall> getAvailableCalls(AccessLevel level) {
			return new List<FunctionCall>(
				from f in bodies where AccessLevelMethods.canAccess(level, f.modifiers.accessLevel, AccessLevelRule.@equals) select new FunctionCall(this, f)
				);
		}

		public Body getBodyFor(List<COOPObject> objects) {

			if (inputsListToBody.TryGetValue(new InputList(objects), out Body b)) {
				if (b.couldExecuteOn(objects)) {
					var output = b;
					if (!b.couldDirectlyExecuteOn(objects)) {
						var fixedBody = b.fixBody(objects);
						Add(fixedBody);
					}

					return output;
				}
			}
			
			foreach (Body body in bodies) {
				if (body.couldExecuteOn(objects)) {
					var output = body;
					if (!body.couldDirectlyExecuteOn(objects)) {
						var fixedBody = body.fixBody(objects);
						Add(fixedBody);
					}

					return output;
				}
			}

			return null;
		}

		public abstract class Usage : CConvertable {

			protected COOPFunction function { get; }
			protected Body body { get; }

			protected Usage(COOPFunction function, Body body) {
				this.function = function;
				this.body = body;
			}
			public abstract string toUsableToC();
		}

		public class Declaration : Usage {
			public Declaration(COOPFunction function, Body body) : base(function, body) { }

			public override string toUsableToC() {
				throw new System.NotImplementedException();
			}
		}

		public class Implementation : Usage {
			public Implementation(COOPFunction function, Body body) : base(function, body) { }

			public override  string toUsableToC() {
				throw new System.NotImplementedException();
			}
		}

		public class Call : Usage {

			private InputList parameterTypes;

			public Call(COOPFunction function, Body body, List<COOPClass> parameterTypes) : base(function, body) {
				this.parameterTypes = new InputList(parameterTypes);
			}

			public override  string toUsableToC() {
				throw new System.NotImplementedException();
			}
		}
		
	}
}