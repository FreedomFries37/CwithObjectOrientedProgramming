using System;
using System.Collections.Generic;
using COOP.core.structures.v2.global.type;

namespace COOP.core.structures.v2.global {
	public class InputList : List<COOPType> {
		public InputList() { }
		public InputList(IEnumerable<COOPType> collection) : base(collection) { }
		public InputList(int capacity) : base(capacity) { }

		public InputList(params COOPObject[] objects) {
			foreach (COOPObject coopObject in objects) {
				Add(coopObject.actualType);
			}
			
		}

		public InputList(List<COOPObject> objects) : this(objects.ToArray()) { }

		protected bool Equals(InputList other) {
			return ListsEquals(this, other);
		}
			
		private static bool ListsEquals(List<COOPType> a, List<COOPType> b) {
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
				foreach (COOPType coopClass in this) {
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