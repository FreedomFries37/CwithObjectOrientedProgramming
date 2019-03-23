namespace COOP.core.structures.v1 {
	public abstract class COOPVariable {

		private string name;
		private COOPClass @class;

		protected COOPVariable(string name, COOPClass @class) {
			this.name = name;
			this.@class = @class;
		}

		public string Name => name;

		public COOPClass Class => @class;
	}
}