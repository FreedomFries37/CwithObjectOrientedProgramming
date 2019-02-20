using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public static COOPClass String;
		private static PrimitiveCOOPClass CharPtr;

		public readonly List<COOPClass> imports = new List<COOPClass>();
		
		static COOPClass(){
			
			COOPFunction init = new COOPFunction("__init__", Base);
			Base = new COOPClass("Object", null, 
				new Collection<COOPFunction>
				{
					init,
					new COOPFunction("ToString", String)
				},
				new Dictionary<string, COOPClass>()
			);
			CharPtr = new PrimitiveCOOPClass("char*");
			String = new COOPClass("String", Base,
				new Collection<COOPFunction>(),
				new Dictionary<string, COOPClass> {
					{"ptr", CharPtr}
				}
			);
			String.addNonDefualtAccessLevel("ptr", AccessLevel.Public);
			Base.functions["ToString"].AccessLevel = AccessLevel.Public;
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
	}
}