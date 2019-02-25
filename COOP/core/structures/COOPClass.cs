using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using COOP.core.structures;

namespace COOP.core.inheritence {
	public class COOPClass {
		
	

		private string name;
		private COOPClass parent;
		private Dictionary<string, COOPFunction> functions;
		private Dictionary<string, COOPClass> varNames;
		private Dictionary<string, AccessLevel> varToLevel = new Dictionary<string, AccessLevel>();

		public static COOPClass Base;
		public static COOPClass Void;
		public static COOPClass String;
		private static PrimitiveCOOPClass CharPtr;

		public readonly List<COOPClass> imports = new List<COOPClass>();

		public bool genFile { get; set; } = true;

		static COOPClass(){
			
			COOPFunction init = new COOPFunction("__init__", Base);
			Void  = new PrimitiveCOOPClass("void");
			Base = new COOPClass("Object", null, 
				new Collection<COOPFunction>
				{
					init,
					new COOPFunction("ToString", String),
					new COOPFunction("Print", Void),
					new COOPFunction("PrintLn", Void)
				},
				new Dictionary<string, COOPClass>()
			);
			Base.functions["ToString"].AccessLevel = AccessLevel.Public;
			Base.functions["ToString"].Body = "return &{.ptr=char[2]{&this, 0}};";
			Base.functions["ToString"].bodyInC = true;
			Base.functions["Print"].AccessLevel = AccessLevel.Public;
			Base.functions["Print"].Body = "ToString().Print()";
			Base.functions["PrintLn"].AccessLevel = AccessLevel.Public;
			Base.functions["PrintLn"].Body = "ToString().PrintLn()";
			Base.functions["__init__"].Body = "";
			Base.functions["__init__"].AccessLevel = AccessLevel.Public;
			Base.functions["__init__"].ReturnType = Base;
			
			CharPtr = new PrimitiveCOOPClass("char*");
			String = new COOPClass("String", Base,
				new Collection<COOPFunction> (),
				new Dictionary<string, COOPClass> {
					{"ptr", CharPtr}
				}
			);
			String.functions.Add("Print", new COOPFunction("Print", Void));
			String.functions.Add("PrintLn", new COOPFunction("PrintLn", Void));
			String.functions.Add("ToString", new COOPFunction("ToString", String));
			
			String.addNonDefualtAccessLevel("ptr", AccessLevel.Protected);
			String.functions["ToString"].AccessLevel = AccessLevel.Public;
			String.functions["ToString"].owner = String;
			String.functions["ToString"].Body = "return this;";
			String.functions["Print"].AccessLevel = AccessLevel.Public;
			String.functions["Print"].owner = String;
			String.functions["Print"].bodyInC = true;
			String.functions["Print"].Body = "\tprintf(\"%s\", __this->ptr);";
			String.functions["PrintLn"].AccessLevel = AccessLevel.Public;
			String.functions["PrintLn"].owner = String;
			String.functions["PrintLn"].bodyInC = true;
			String.functions["PrintLn"].Body = "\tprintf(\"%s\\n\", __this->ptr);";

			Base.functions["ToString"].ReturnType = String;
		}

		public COOPClass(string name, COOPClass parent, Dictionary<string, COOPFunction> functions, Dictionary<string, COOPClass> varNames) {
			this.name = name;
			this.parent = parent;
			this.functions = functions;
			this.varNames = varNames;
			foreach (COOPFunction functionsValue in functions.Values) {
				functionsValue.owner = this;
			}
		}
		
		public COOPClass(string name, COOPClass parent, Collection<COOPFunction> functions, Dictionary<string, COOPClass> varNames) {
			this.name = name;
			this.parent = parent;
			this.functions = new Dictionary<string, COOPFunction>();
			foreach (COOPFunction coopFunction in functions) {
				coopFunction.owner = this;
				this.functions.Add(coopFunction.Name, coopFunction);
			}
			this.varNames = varNames;
		}

		public void addNonDefualtAccessLevel(string var, AccessLevel level) {
			varToLevel.Add(var, level);
			//functions[var].AccessLevel = level;
		}

		public AccessLevel getAccessLevel(string var) {
			if (varToLevel.ContainsKey(var)) return varToLevel[var];
			return 0;
		}

		public string Name => name;

		public COOPClass Parent => parent;

		public Dictionary<string, COOPFunction> Functions => functions;

		public Dictionary<string, COOPClass> VarNames => varNames;

		public override string ToString() {
			return name;
		}

		public virtual string convertToC() {
			return "struct " + name + "*";
		}

		public void addImports(COOPClass coopClass) {
			imports.Add(coopClass);
		}

		public List<COOPFunction> getFunctions() {
			List<COOPFunction> output = new List<COOPFunction>();
			output.AddRange(from f in Functions.Keys select Functions[f]);
			return output;
		}

		public bool isStatic(string function) {
			return functions[function].IsStatic;
		}

		protected bool Equals(COOPClass other) {
			return string.Equals(name, other.name);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((COOPClass) obj);
		}

		public override int GetHashCode() {
			return (name != null ? name.GetHashCode() : 0);
		}
	}
}