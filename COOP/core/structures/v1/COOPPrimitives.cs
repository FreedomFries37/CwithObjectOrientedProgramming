namespace COOP.core.structures.v1 {
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
			
			@ulong = new PrimitiveCOOPClass("unsigned_long");
			uinteger = new PrimitiveCOOPClass("unsigned_int");
			@ushort = new PrimitiveCOOPClass("unsigned_short");
			@ubyte = new PrimitiveCOOPClass("unsigned_byte");
			
			@float = new PrimitiveCOOPClass("float");
			@double = new PrimitiveCOOPClass("double");
		}
	}
}