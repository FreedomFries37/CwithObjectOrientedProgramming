namespace FileCondenser.core {
	public class HuffmanTreeLeaf : HuffmanTreeNode{
		
		public char representing { get; }
		public long quantity { get; }

		public HuffmanTreeLeaf(char representing, long quantity) : base(null, null) {
			this.representing = representing;
			this.quantity = quantity;
		}

		public override long GetQuantity() {
			return quantity;
		}
	}
}