using System.Collections;
using System.Collections.Generic;

namespace FileCondenser.core.expand {
	public class BitOutputStream : IEnumerable<bool> {
		private readonly List<bool> bits;

		public BitOutputStream(IEnumerable<char> chars) {
			bits = new List<bool>();

			foreach (var c in chars) {
				var b = (byte) c;
				for (var i = 0; i < 8; i++) {
					var m = (b >> (7 - i)) & 0x1;
					bits.Add(m == 1);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<bool> GetEnumerator() {
			return bits.GetEnumerator();
		}
	}
}