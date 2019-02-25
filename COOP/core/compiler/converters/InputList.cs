using System;
using System.Collections.Generic;
using COOP.core.inheritence;

namespace COOP.core.compiler.converters {
	public class InputList : List<COOPClass> {
		public InputList() { }
		public InputList(IEnumerable<COOPClass> collection) : base(collection) { }
		public InputList(int capacity) : base(capacity) { }

		protected bool Equals(InputList other) {
			return ListsEquals(this, other);
		}
			
		private static bool ListsEquals(List<COOPClass> a, List<COOPClass> b) {
			//if (List<COOPClass>.Equals(a, b)) return true;
			if (a.Count != b.Count) return false;
			for (var i = 0; i < a.Count; i++) {
				if (!a[i].Equals(b[i])) return false;
			}

			return true;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InputList) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int output = 0;
				foreach (COOPClass coopClass in this) {
					output += coopClass.Name.GetHashCode() * 397;
				}

				return output;
			}
		}


		public override string ToString() {
			return "{" + String.Join(",", this) + "}";
		}
	}
}