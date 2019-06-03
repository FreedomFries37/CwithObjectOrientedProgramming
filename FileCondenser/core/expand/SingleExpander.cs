using System;
using System.Collections.Generic;
using System.Text;

namespace FileCondenser.core.expand {
	public class SingleExpander {
		public string Expand(string w, HuffmanChain chain, long totalchars) {
			var bOS = new BitOutputStream(w);
			var bools = new Queue<bool>(bOS);
			var builder = new StringBuilder();
			var previousChar = '\0';
			for (long i = 0; i < totalchars; i++) {
				var path = new StringBuilder();
				var added = false;
				var maxDepth = i == 0 ? chain.MaxDepth() : chain.MaxDepth(previousChar);

				var charToAdd = '\0';

				if (maxDepth == 0) {
					if (i == 0)
						added = chain.TryGet("", out charToAdd);
					else
						added = chain.TryGet("", previousChar, out charToAdd);
				}

				while (!added) {
					path.Append(bools.Dequeue() ? '1' : '0');

					if (path.Length > maxDepth) throw new Exception("Invalid Path");

					if (i == 0)
						added = chain.TryGet(path.ToString(), out charToAdd);
					else
						added = chain.TryGet(path.ToString(), previousChar, out charToAdd);
				}

				builder.Append(charToAdd);
				previousChar = charToAdd;
			}


			return builder.ToString();
		}
	}
}