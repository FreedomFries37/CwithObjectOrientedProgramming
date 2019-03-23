using System.Collections.Generic;
using System.Linq;
using COOP.core.structures.v2.functions;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type.included;

namespace COOP.core.structures.v2.global.type {
	public class COOPAbstract : COOPInterface{

		public COOPAbstract parent { get; }

		public Dictionary<string, Field> fields { get; }

		public COOPAbstract(string name) : this(name, IncludedClasses.Object) { }

		public COOPAbstract(string name, COOPAbstract parent) : base(name) {
			this.parent = parent;
			fields = new Dictionary<string, Field>();
		}

		public bool addField(Field f) {
			if (fields.ContainsKey(f.name)) return false;
			fields.Add(f.name, f);
			return true;
		}

		public bool isParent(COOPAbstract type) {
			if (this ==type) return true;
			if (parent == null) return false;
			if (type == null) return false;
			return parent.isParent(type);
		}

		public override bool isParent(COOPType type) {
			return isParent(type as COOPInterface) || isParent(type as COOPAbstract);
		}

		public override List<FunctionCall> getAvailableFunctions(AccessLevel accessLevel) {
			List<FunctionCall> availableFunctions = base.getAvailableFunctions(accessLevel);
			availableFunctions.AddRange(parent.getAvailableFunctions(AccessLevel.Protected));
			return availableFunctions;
		}
		
		public Field[] getFields(AccessLevel accessLevel, AccessLevelRule rule) {
			List<Field> output = new List<Field>();
			if(parent != null) output.AddRange(parent.getFields(accessLevel, rule));
			foreach (Field fieldsValue in fields.Values) {
				if(AccessLevelMethods.canAccess(accessLevel, fieldsValue.modifiers.accessLevel))
					output.Append(fieldsValue);
			}

			return output.ToArray();
		}
		
		public override bool isStrictlyInterface() => false;

		public override bool isStrictlyAbstract() => true;
	}

	
}