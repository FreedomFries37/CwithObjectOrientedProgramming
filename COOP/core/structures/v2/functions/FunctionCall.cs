using COOP.core.structures.v2.functions.function_bodies;
using global::COOP.core.structures.v2.global;

namespace COOP.core.structures.v2.functions {
	public class FunctionCall {

		public COOPFunction function { get; }
		public InputList inputList { get; }
		public Body body { get; }

		public FunctionCall(COOPFunction function, Body body) {
			this.function = function;
			this.inputList = body.inputList();
			this.body = body;
		}

		protected bool Equals(FunctionCall other) {
			return Equals(function, other.function) && Equals(inputList, other.inputList) && Equals(body, other.body);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((FunctionCall) obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (function != null ? function.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (inputList != null ? inputList.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (body != null ? body.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}