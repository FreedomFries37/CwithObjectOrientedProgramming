namespace COOP.core.structures.v2.global.type {
	public abstract class COOPType {
		public string Name { get; }

		public COOPType(string name) {
			Name = name;
		}

		public abstract bool isParent(COOPType type);

		public virtual bool isStrictlyInterface() => false;
		public virtual bool isStrictlyAbstract() => false;
		public virtual bool isStrictlyClass() => false;
	}
}