using System.Collections.Generic;

namespace FileCondenser.core {
	public class HuffmanChain {

		private Dictionary<char, HuffmanTree> trees;
		private HuffmanTree overall;

		public string this[char c] => overall[c];
		public string this[char c, char before] => trees[before][c];

		public HuffmanChain(string w) {
			overall = HuffmanTree.AssembleTree(CharCountDeterminer.GetAllFromString(w));
			
			
			trees = new Dictionary<char, HuffmanTree>();
			foreach (var c in GetAllChars(w)) {
				var afterChar = CharCountDeterminer.GetCountsFromStringForAfterChar(w, c);
				trees.Add(c, HuffmanTree.AssembleTree(afterChar));
			}
		}

		private IEnumerable<char> GetAllChars(string w) {
			HashSet<char> found = new HashSet<char>();
			foreach (var c in w.ToCharArray()) {
				if (!found.Contains(c)) {
					found.Add(c);
				}
			}

			return found;
		}
	}
}