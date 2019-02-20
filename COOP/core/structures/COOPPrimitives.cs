using COOP.core.inheritence;

namespace COOP.core.structures {
	public partial class COOPPrimitives {
		public static COOPClass @long;
		public static COOPClass integer;
		public static COOPClass @short;
		public static COOPClass @byte;
		// public static COOPClass boolean;
		
		public static COOPClass @ulong;
		public static COOPClass uinteger;
		public static COOPClass @ushort;
		public static COOPClass @ubyte;
		
		public static COOPClass @float;
		public static COOPClass @double;

		static COOPPrimitives (){
			@long = new PrimitiveCOOPClass("long");
			integer = new PrimitiveCOOPClass("int");
			@short = new PrimitiveCOOPClass("short");
			@byte = new PrimitiveCOOPClass("byte");
			
			@ulong = new PrimitiveCOOPClass("unsigned long");
			integer = new PrimitiveCOOPClass("unsigned int");
			@short = new PrimitiveCOOPClass("unsigned short");
			@byte = new PrimitiveCOOPClass("unsigned byte");
			
			@float = new PrimitiveCOOPClass("float");
			@double = new PrimitiveCOOPClass("double");
		}
	}
}