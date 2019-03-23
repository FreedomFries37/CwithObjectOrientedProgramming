namespace COOP.core.structures.v2.global.modifiers {
	public enum AccessLevel {
		@Sealed = 0, // only compiler knows it exists
		@Private = 1,
		@Protected = 2,
		@Directory = 3, // if in same directory
		@Public = 4
	}
}