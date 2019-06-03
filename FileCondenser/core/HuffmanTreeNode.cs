using System;
using System.Runtime.Serialization;

namespace FileCondenser.core {
	public class HuffmanTreeNode : IComparable<HuffmanTreeNode> {
		public HuffmanTreeNode(HuffmanTreeNode left, HuffmanTreeNode right) {
			this.left = left;

			if (this.left != null) this.left.parent = this;

			this.right = right;

			if (this.right != null) this.right.parent = this;

			parent = null;
		}

		internal HuffmanTreeNode left { get; }
		internal HuffmanTreeNode right { get; }

		private HuffmanTreeNode parent { get; set; }

		public long Quantity => GetQuantity();

		public int CompareTo(HuffmanTreeNode other) {
			if (other == null) return 1;
			if (other.GetQuantity() == GetQuantity()) return 0;
			return GetQuantity() > other.GetQuantity() ? 1 : -1;
		}


		public virtual long GetQuantity() {
			var leftSum = left?.GetQuantity() ?? 0;
			var rightSum = right?.GetQuantity() ?? 0;
			return leftSum + rightSum;
		}

		public string GetPath() {
			if (parent == null) return "";

			// this is left
			if (parent.left == this) return parent.GetPath() + '0';
			if (parent.right == this) return parent.GetPath() + '1';
			throw new HuffmanNodePathException();
		}

		internal HuffmanTreeNode GetHead() {
			if (parent == null) return this;
			return parent.GetHead();
		}

		[Serializable]
		public class HuffmanNodePathException : Exception {
			//
			// For guidelines regarding the creation of new exception types, see
			//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
			// and
			//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
			//

			public HuffmanNodePathException() { }
			public HuffmanNodePathException(string message) : base(message) { }
			public HuffmanNodePathException(string message, Exception inner) : base(message, inner) { }

			protected HuffmanNodePathException(
				SerializationInfo info,
				StreamingContext context) : base(info, context) { }
		}
	}
}