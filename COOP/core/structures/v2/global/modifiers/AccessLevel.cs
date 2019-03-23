using System;

namespace COOP.core.structures.v2.global.modifiers {
	public enum AccessLevel {
		@Sealed = 0, // only compiler knows it exists
		@Private = 1,
		@Protected = 2,
		@Directory = 3, // if in same directory
		@Public = 4
		
	}

	public enum AccessLevelRule {
		equals,
		inLessThanEqualTo
	}
	public class AccessLevelMethods {
		public static bool canAccess(AccessLevel inAccess, AccessLevel objectAccesLevel, AccessLevelRule rule = AccessLevelRule.inLessThanEqualTo) {
			if(rule == AccessLevelRule.inLessThanEqualTo)
				return inAccess <= objectAccesLevel;
			if (rule == AccessLevelRule.@equals) {
				return inAccess == objectAccesLevel;
			}

			throw new Exception("No access rule specified");
		}
	}
}