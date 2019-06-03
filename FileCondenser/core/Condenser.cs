using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCondenser.core {
	public class Condenser {
		private readonly Dictionary<string, HuffmanChain> _chains;
		private readonly List<string> paths;

		public Condenser(List<string> paths) {
			this.paths = paths;
			_chains = new Dictionary<string, HuffmanChain>();
		}

		public Condenser(params string[] paths) {
			this.paths = paths.ToList();
			_chains = new Dictionary<string, HuffmanChain>();
		} //

		public string Condense() {
			foreach (var path in paths) {
				var text = File.ReadAllText(path);
				var (s, chain) = Condense(text);
				_chains.Add(path, chain);
			}

			return "";
		}

		protected (string, HuffmanChain) Condense(string w) {
			var chain = new HuffmanChain(w);

			var b = new BitStream();
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
	}
}