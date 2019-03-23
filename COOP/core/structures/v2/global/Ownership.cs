namespace COOP.core.structures.v2.global {
	public class Ownership<T> {

		public bool isOwned { get; }
		public T owner { get; }

		public Ownership(T owner, bool isOwned) {
			this.owner = owner;
			this.isOwned = isOwned;
		}

		public static Ownership<object> noOwnershipe() {
			return new Ownership<object>(null, false);
		}

		public static Ownership<T> ownership(T parent) {
			return new Ownership<T>(parent, true);
		}
	}
}