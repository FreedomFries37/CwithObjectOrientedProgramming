using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions.statements {
	public abstract class Statement : CConvertable{

		public Ownership<COOPFunction> ownership { get; }

		protected Statement(Ownership<COOPFunction> ownership) {
			this.ownership = ownership;
		}

		public abstract bool validate();
		public abstract string toUsableToC();
		public override string ToString() {
			return toUsableToC();
		}
	}
}