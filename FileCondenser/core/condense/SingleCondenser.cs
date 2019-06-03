using System.Text;

namespace FileCondenser.core {
	public class SingleCondenser {
		public (string, HuffmanChain) Condense(string w) {
			var chain = new HuffmanChain(w);
			if (w.Length == 0) return ("", chain);


			var b = new BitInputStream();
			var charArray = w.ToCharArray();

			char first = charArray[0], pre = first;
			b.AddFromStringSource(chain[first]);

			for (var i = 1; i < charArray.Length; i++) {
				var current = charArray[i];

				var relPath = chain[current, pre];
				b.AddFromStringSource(relPath);

				pre = current;
			}

			var output = new StringBuilder();
			b.Flush();
			output.Append(b.ToChars());

			return (output.ToString(), chain);
		}

		public (string, HuffmanTree) CondenseToTree(string w) {
			var tree = HuffmanTree.AssembleTree(CharCountDeterminer.GetAllFromString(w));

			var b = new BitInputStream();
			var charArray = w.ToCharArray();

			var first = charArray[0];
			b.AddFromStringSource(tree[first]);

			for (var i = 1; i < charArray.Length; i++) {
				var current = charArray[i];

				var relPath = tree[current];
				b.AddFromStringSource(relPath);
			}

			var output = new StringBuilder();
			b.Flush();
			output.Append(b.ToChars());

			return (output.ToString(), tree);
		}
	}
}