using System;
using System.Collections.Generic;
using System.Linq;


namespace FileCondenser.core {
	public class HuffmanTree {

		private Dictionary<char, HuffmanTreeLeaf> map { get; }

		public string this[char c] => map[c].GetPath();

		protected HuffmanTree(Dictionary<char, HuffmanTreeLeaf> map) {
			this.map = map;
		}

		protected internal class NodeComparer : IComparer<HuffmanTreeNode> {
			public int Compare(HuffmanTreeNode x, HuffmanTreeNode y) {
				if (x == null) return -1;
				return x.CompareTo(y);
			}
		}

		public static HuffmanTree AssembleTree(IDictionary<char, long> pairs) => AssembleTree((IEnumerable<KeyValuePair<char, long>>) pairs);
		
		public static HuffmanTree AssembleTree(IEnumerable<KeyValuePair<char, long>> pairs) =>
			AssembleTree(from f in pairs select (f.Key, f.Value));

		public static HuffmanTree AssembleTree(IList<KeyValuePair<char, long>> pairs) => AssembleTree((IEnumerable<KeyValuePair<char, long>>) pairs);

		public static HuffmanTree AssembleTree(IList<(char c, long q)> pairs) =>
			AssembleTree((IEnumerable<(char c, long q)>) pairs);
		
		public static HuffmanTree AssembleTree(IEnumerable<(char c, long q)> pairs) {
			var map = new Dictionary<char, HuffmanTreeLeaf>();
			IHeap<HuffmanTreeNode> nodeHeap = new Heap<HuffmanTreeNode>(new NodeComparer());
			
			foreach (var valueTuple in pairs) {
				(char c, long q) = valueTuple;
				HuffmanTreeLeaf leaf = new HuffmanTreeLeaf(c, q);
				
				map.Add(c, leaf);
				nodeHeap.Add(leaf);
			}

			while (!nodeHeap.IsEmpty()) {
				var nodeA = nodeHeap.Pop();
				if (nodeHeap.IsEmpty()) break;

				var nodeB = nodeHeap.Pop();
				
				var newNode = new HuffmanTreeNode(nodeA, nodeB);
				nodeHeap.Add(newNode);
			}


			if (!EnsureAllSameHead(map.Values)) return null;
			
			return new HuffmanTree(map);
		}


		
		
		private static bool EnsureAllSameHead(IEnumerable<HuffmanTreeLeaf> leaves) {
			HuffmanTreeNode parent = null;
			foreach (var huffmanTreeLeaf in leaves) {
				if (parent == null) parent = huffmanTreeLeaf.GetHead();
				else if (huffmanTreeLeaf.GetHead() != parent) return false;
			}

			return true;
		}
	}
}