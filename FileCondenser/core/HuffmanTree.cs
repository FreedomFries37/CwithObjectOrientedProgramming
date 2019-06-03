using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileCondenser.core {
	public class HuffmanTree {
		private HuffmanTree(Dictionary<char, HuffmanTreeLeaf> map) {
			this.map = map;
		}

		private Dictionary<char, HuffmanTreeLeaf> map { get; }
		private HuffmanTreeNode head { get; set; }

		public string this[char c] => map[c].GetPath();


		internal ICollection<char> Keys => ((IDictionary<char, HuffmanTreeLeaf>) map).Keys;

		public static HuffmanTree AssembleTree(IDictionary<char, long> pairs) {
			return AssembleTree((IEnumerable<KeyValuePair<char, long>>) pairs);
		}

		public static HuffmanTree AssembleTree(IEnumerable<KeyValuePair<char, long>> pairs) {
			return AssembleTree(from f in pairs select (f.Key, f.Value));
		}

		public static HuffmanTree AssembleTree(IList<KeyValuePair<char, long>> pairs) {
			return AssembleTree((IEnumerable<KeyValuePair<char, long>>) pairs);
		}

		public static HuffmanTree AssembleTree(IList<(char c, long q)> pairs) {
			return AssembleTree((IEnumerable<(char c, long q)>) pairs);
		}

		public static HuffmanTree AssembleTree(IEnumerable<(char c, long q)> pairs) {
			var map = new Dictionary<char, HuffmanTreeLeaf>();
			IHeap<HuffmanTreeNode> nodeHeap = new Heap<HuffmanTreeNode>(new NodeComparer());

			foreach (var valueTuple in pairs) {
				var (c, q) = valueTuple;
				var leaf = new HuffmanTreeLeaf(c, q);

				map.Add(c, leaf);
				nodeHeap.Add(leaf);
			}

			HuffmanTreeNode head = null;
			while (!nodeHeap.IsEmpty()) {
				var nodeA = nodeHeap.Pop();
				if (nodeHeap.IsEmpty()) {
					head = nodeA.GetHead();
					break;
				}

				var nodeB = nodeHeap.Pop();

				var newNode = new HuffmanTreeNode(nodeA, nodeB);
				nodeHeap.Add(newNode);
			}


			if (!EnsureAllSameHead(map.Values)) return null;

			var output = new HuffmanTree(map) {
				head = head
			};
			return output;
		}

		internal long GetQuantity(char key) {
			return map.GetValueOrDefault(key, null)?.Quantity ?? 0;
		}

		private static bool EnsureAllSameHead(IEnumerable<HuffmanTreeLeaf> leaves) {
			HuffmanTreeNode parent = null;
			foreach (var huffmanTreeLeaf in leaves)
				if (parent == null) parent = huffmanTreeLeaf.GetHead();
				else if (huffmanTreeLeaf.GetHead() != parent) return false;

			return true;
		}

		public bool TryGet(string path, out char c) {
			c = (char) 0;

			var node = head;
			var dirArray = path.ToCharArray();
			foreach (var dir in dirArray) {
				switch (dir) {
					case '0': {
						node = node.left;
						break;
					}

					case '1': {
						node = node.right;
						break;
					}

					default:
						throw new InvalidDataException("Not proper huffman encoding");
				}

				if (node == null) return false;
			}

			var leaf = node as HuffmanTreeLeaf;
			if (leaf == null) return false;
			c = leaf.representing;
			return true;
		}

		public int MaxDepth() {
			return MaxDepth(head) - 1;
		}

		private int Max(int a, int b) {
			return a > b ? a : b;
		}

		private int MaxDepth(HuffmanTreeNode node) {
			if (node == null) return 0;
			var max = 0;
			max = Max(max, MaxDepth(node.left));
			max = Max(max, MaxDepth(node.right));
			return max + 1;
		}

		public bool Equals(HuffmanTree other) {
			if (other.Keys.Except(Keys).Any()) return false;
			foreach (var key in Keys)
				if (GetQuantity(key) != other.GetQuantity(key))
					return false;

			return true;
		}

		private class NodeComparer : IComparer<HuffmanTreeNode> {
			public int Compare(HuffmanTreeNode x, HuffmanTreeNode y) {
				if (x == null) return 1;
				return -x.CompareTo(y);
			}
		}
	}
}