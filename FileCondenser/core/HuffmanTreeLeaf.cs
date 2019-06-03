namespace FileCondenser.core {
	public class HuffmanTreeLeaf : HuffmanTreeNode {
		public HuffmanTreeLeaf(char representing, long quantity) : base(null, null) {
			this.representing = representing;
			this.quantity = quantity;
		}

		public char representing { get; }
		private long quantity { get; }

		public override long GetQuantity() {
			return quantity;
		}
	}
}