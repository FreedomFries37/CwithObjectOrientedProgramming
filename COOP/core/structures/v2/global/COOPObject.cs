using COOP.core.structures.v2.global.type;

namespace COOP.core.structures.v2.global {
	public class COOPObject {
		public COOPType type { get; }
		public COOPClass actualType { get; }

		public COOPObject(COOPClass type) {
			this.type = actualType = type;
			
		}

		public COOPObject(COOPType type, COOPClass actualType) {
			this.type = type;
			this.actualType = actualType;
		}


		public COOPObject COOPCast<T>(T next) where T : COOPType {
			if (next is COOPAbstract) {
				COOPAbstract coopAbstract = next as COOPAbstract;
				if(actualType.isParent(coopAbstract))
					return new COOPObject(next, actualType);
			}
			if(actualType.isParent(next))
				return new COOPObject(next, actualType);

			return null;
		}
		
		
	}
}