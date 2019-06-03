using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCondenser.core {
	public class Condenser {
		private List<string> paths;
		private Dictionary<string, HuffmanChain> _chains;

		public Condenser(List<string> paths) {
			this.paths = paths;
			_chains = new Dictionary<string, HuffmanChain>();
		}

		public Condenser(params string[] paths) {
			this.paths = paths.ToList();
			_chains = new Dictionary<string, HuffmanChain>();
		}

		public string Condense() {
			foreach (string path in paths) {
				string text = File.ReadAllText(path);
				(string s, HuffmanChain chain) = Condense(text);
				_chains.Add(path, chain);
			}

			return "";
		}

		protected (string, HuffmanChain) Condense(string w) {
			HuffmanChain chain = new HuffmanChain(w);
			
			BitStream b = new BitStream();
			char[] charArray = w.ToCharArray();

			char first = charArray[0], pre = first;
			b.AddFromStringSource(chain[first]);
			
			for (var i = 1; i < charArray.Length; i++) {
				char current = charArray[i];

				string relPath = chain[current, pre];
				b.AddFromStringSource(relPath);

				pre = current;
			}
			
			StringBuilder output = new StringBuilder();
			b.Flush();
			output.Append(b.ToChars());

			return (output.ToString(), chain);
		}
	}
}