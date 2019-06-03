using System.Collections.Generic;

namespace FileCondenser.core {
	public static class CharCountDeterminer {

		public static Dictionary<char, long> GetAllFromString(string w) {
			var output = new Dictionary<char, long>();

			for (var i = 0; i < w.Length; i++) {
				char c = w[i];
				if (!output.ContainsKey(c)) {
					output.Add(c, 0);
				}

				output[c]++;
			}

			

			return output;
		}
		
		public static Dictionary<char, long> GetCountsFromStringForAfterChar(string w, char pre) {
			var output = new Dictionary<char, long>();

			for (var i = 1; i < w.Length; i++) {
				char c = w[i];
				char before = w[i - 1];

				if (before == pre) {
					if (!output.ContainsKey(c)) {
						output.Add(c, 0);
					}

					output[c]++;
				}
			}

			

			return output;
		}
	}
}