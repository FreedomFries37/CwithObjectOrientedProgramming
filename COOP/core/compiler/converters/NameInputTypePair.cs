using System;
using System.Collections.Generic;
using COOP.core.inheritence;

namespace COOP.core.compiler.converters {
	public class NameInputTypePair {
		public string name { get; }
		public InputList inputs { get; }

		public NameInputTypePair(string name, List<COOPClass> inputs) {
			this.name = name;
			this.inputs = new InputList(inputs);
		}

		protected bool Equals(NameInputTypePair other) {
			return String.Equals(name, other.name) && Equals(inputs, other.inputs);
		}

			

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NameInputTypePair) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((name != null ? name.GetHashCode() : 0) * 397) ^ (inputs != null ? inputs.GetHashCode() : 0);
			}
		}

		private sealed class NameInputsEqualityComparer : IEqualityComparer<NameInputTypePair> {
			public bool Equals(NameInputTypePair x, NameInputTypePair y) {
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return String.Equals(x.name, y.name) && x.inputs.Equals(y.inputs);
			}

			public int GetHashCode(NameInputTypePair obj) {
				unchecked {
					return obj.GetHashCode();
				}
			}
		}

		public static IEqualityComparer<NameInputTypePair> nameInputsComparer { get; } = new NameInputsEqualityComparer();

		public override string ToString() {
			return "{" + inputs + ", " + name + "}";
		}
	}
}