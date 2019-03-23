using System;

namespace COOP.core.structures.v2.exceptions {
	public class NoCConversionPossibleException : Exception{
		public NoCConversionPossibleException() : base("No C translation possible.") { }
	}
}