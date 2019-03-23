namespace COOP.core.structures.v2.global.modifiers {
	public class Modifiers {

		public bool isExternallyImplemented { get; }
		public AccessLevel accessLevel { get; }
		public bool isStatic { get; }

		public Modifiers(AccessLevel accessLevel, bool isStatic, bool isExternallyImplemented = false) {
			this.accessLevel = accessLevel;
			this.isStatic = isStatic;
			this.isExternallyImplemented = isExternallyImplemented;
		}
		
		public Modifiers(bool isStatic, bool isExternallyImplemented = false) {
			accessLevel = AccessLevel.Private;
			this.isStatic = isStatic;
			this.isExternallyImplemented = isExternallyImplemented;
		}
	}
}