using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FileCondenser.core.input;
using FileCondenser.core.output;

namespace FileCondenser.core {
	public class HuffmanChain {
		private static readonly HuffmanChainInputService _inputService = new HuffmanChainInputService();
		private readonly HuffmanChainOutputService _outputService = new HuffmanChainOutputService();
		private readonly HuffmanTree overall;

		private readonly Dictionary<char, HuffmanTree> trees;

		public HuffmanChain(string w) {
			overall = HuffmanTree.AssembleTree(CharCountDeterminer.GetAllFromString(w));


			trees = new Dictionary<char, HuffmanTree>();
			foreach (var c in GetAllChars(w)) {
				var afterChar = CharCountDeterminer.GetCountsFromStringForAfterChar(w, c);
				trees.Add(c, HuffmanTree.AssembleTree(afterChar));
			}
		}

		private HuffmanChain(Dictionary<char, HuffmanTree> trees, HuffmanTree overall) {
			this.trees = trees;
			this.overall = overall;
		}


		public string this[char c] => overall[c];
		public string this[char c, char before] => trees[before][c];

		private IEnumerable<char> GetAllChars(string w) {
			var found = new HashSet<char>();
			foreach (var c in w.ToCharArray())
				if (!found.Contains(c))
					found.Add(c);

			return found;
		}

		public bool TryGet(string path, out char c) {
			return overall.TryGet(path, out c);
		}

		public bool TryGet(string path, char previous, out char c) {
			return trees[previous].TryGet(path, out c);
		}

		public int MaxDepth() {
			return overall.MaxDepth();
		}

		public int MaxDepth(char c) {
			return trees[c].MaxDepth();
		}

		private long GetQuantity(char c) {
			return overall.GetQuantity(c);
		}

		private long GetQuantity(char c, char previous) {
			return trees.GetValueOrDefault(previous, null)?.GetQuantity(c) ?? 0;
		}

		public bool Equals(HuffmanChain other) {
			if (!other.overall.Equals(overall)) return false;
			if (other.trees.Keys.Except(trees.Keys).Any()) return false;

			foreach (var treesKey in trees.Keys)
				if (!trees[treesKey].Equals(other.trees[treesKey]))
					return false;

			return true;
		}


		public override string ToString() {
			return _outputService.CreateOutput(this);
		}

		public static HuffmanChain Reconstruct(string w) {
			return _inputService.CreateFromInput(w);
		}

		public class HuffmanChainOutputService : IOutputService<HuffmanChain> {
			public string CreateOutput(HuffmanChain tObject) {
				var builder = new StringBuilder();

				builder.Append('{');
				var overallKeys = tObject.overall.Keys;
				foreach (var key in overallKeys) {
					var overallQ = tObject.GetQuantity(key);
					var preQs = new (char pre, long preQ)[overallKeys.Count];

					var index = 0;
					foreach (var preKey in overallKeys) preQs[index++] = (preKey, tObject.GetQuantity(preKey, key));

					var keyString =
						$"{KeyInfo(key, overallQ)}{string.Join("", from preQ in preQs select KeyInfo(preQ.pre, preQ.preQ))}@~";
					builder.Append(keyString);
				}

				builder.Append('}');
				return builder.ToString();
			}

			private string KeyInfo(char key, long amount) {
				var bytes = BitConverter.GetBytes(amount);
				var amountAsHex = "";

				for (var i = 0; i < bytes.Length; i += 2) {
					var e = (char) ((bytes[i] << 8) | bytes[i + 1]);
					if (e > 0)
						amountAsHex += e;
				}


				return $"{key}{amountAsHex}@#";
			}
		}

		public class HuffmanChainInputService : IInputService<HuffmanChain> {
			public HuffmanChain CreateFromInput(string w) {
				if (w[0] != '{' ||
					w[w.Length - 1] != '}') throw new InvalidDataContractException();
				var fixedW = w.Remove(0, 1);
				fixedW = fixedW.Remove(fixedW.Length - 1);

				var overallCount = new Dictionary<char, long>();
				var trees = new Dictionary<char, HuffmanTree>();

				var lines = (from l in fixedW.Split("@~") where l.Length > 0 select l).ToArray();
				foreach (var line in lines) {
					var keyInfos = (from kv in line.Split("@#") where kv.Length > 0 select kv).ToArray();
					var preTree = new Dictionary<char, long>();

					var overall = GetKeyInfo(keyInfos[0]);
					var mainChar = overall.c;

					overallCount.Add(mainChar, overall.quantity);

					for (var i = 1; i < keyInfos.Length; i++) {
						var afterInfo = GetKeyInfo(keyInfos[i]);

						var afterChar = afterInfo.c;
						var afterQuantity = afterInfo.quantity;

						if (afterQuantity > 0)
							preTree.Add(afterChar, afterQuantity);
					}

					var tree = HuffmanTree.AssembleTree(preTree);
					trees.Add(mainChar, tree);
				}


				var overallTree = HuffmanTree.AssembleTree(overallCount);
				return new HuffmanChain(trees, overallTree);
			}

			private (char c, long quantity) GetKeyInfo(string w) {
				var outputC = w[0];
				var quantityStr = w.Remove(0, 1);

				var bytes = new List<byte>();
				for (var i = 0; i < quantityStr.Length; i++) {
					var iBytes = BitConverter.GetBytes(quantityStr[i]);

					bytes.AddRange(iBytes.Reverse());
				}

				for (var i = 0; i < 8 - quantityStr.Length; i++) bytes.Add(0);

				var outputQuantity = BitConverter.ToInt64(bytes.ToArray());
				return (outputC, outputQuantity);
			}
		}
	}
}