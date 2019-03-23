using COOP.core.structures.v2.global.modifiers;

namespace COOP.core.structures.v2.global.type {
	public class Field{

		public Modifiers modifiers { get; }
		public COOPType type { get; }
		public string name { get; }

		public Field(Modifiers modifiers, COOPType type, string name) {
			this.modifiers = modifiers;
			this.type = type;
			this.name = name;
		}

		
	}
}