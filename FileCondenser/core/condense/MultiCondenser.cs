using System.Collections.Generic;
using System.IO;

namespace FileCondenser.core {
	public class MultiCondenser : ICondenser {
		private readonly Dictionary<string, HuffmanChain> _chains;
		private readonly Dictionary<string, string> _condensed;
		private readonly List<string> paths;

		private readonly SingleCondenser _singleCondenser;

		public MultiCondenser(List<string> paths) {
			this.paths = paths;
			_chains = new Dictionary<string, HuffmanChain>();
			_singleCondenser = new SingleCondenser();
			_condensed = new Dictionary<string, string>();
		}

		public MultiCondenser(params string[] paths) : this(new List<string>(paths)) { } //

		public (string condensed, HuffmanChain chain) this[string path] => (_condensed[path], _chains[path]);


		public (string, HuffmanChain) Condense(string path) {
			(string w, HuffmanChain chain) o = _singleCondenser.Condense(File.ReadAllText(path));

			if (!_chains.ContainsKey(path))
				_chains.Add(path, o.chain);
			else
				_chains[path] = o.chain;

			if (!_condensed.ContainsKey(path))
				_condensed.Add(path, o.w);
			else
				_condensed[path] = o.w;

			return o;
		}

		public void Add(string path) {
			paths.Add(path);
		}

		public string GetCondensed(string path) {
			if (!path.Contains(path)) throw new InvalidDataException("Invalid File Path");
			if (!_condensed.ContainsKey(path)) Condense(path);

			return _condensed[path];
		}

		public HuffmanChain GetChain(string path) {
			if (!path.Contains(path)) throw new InvalidDataException("Invalid File Path");

			if (!_chains.ContainsKey(path)) Condense(path);

			return _chains[path];
		}

		public void SetChainAndCondensed(string path, string condensed, HuffmanChain chain) {
			if (!path.Contains(path)) throw new InvalidDataException("Invalid File Path");

			if (!_chains.ContainsKey(path))
				_chains.Add(path, chain);
			else
				_chains[path] = chain;

			if (!_condensed.ContainsKey(path))
				_condensed.Add(path, condensed);
			else
				_condensed[path] = condensed;
		}
	}
}