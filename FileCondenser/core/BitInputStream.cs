using System;
using System.Collections.Generic;
using System.IO;

namespace FileCondenser {
	public class BitInputStream {
		private readonly List<byte> bytes;
		private byte currentByte;
		private int index;

		public BitInputStream() {
			bytes = new List<byte>();
		}

		public void Add(bool b) {
			var f = currentByte;
			var _b = (byte) (b ? 0b1 : 0b0);

			_b <<= 7 - index++;
			f = (byte) (_b | f);
			if (index == 8) {
				bytes.Add(f);
				index = 0;
				currentByte = 0;
			} else {
				currentByte = f;
			}
		}

		public void AddFromStringSource(string s) {
			var charArray = s.ToCharArray();
			for (var i = 0; i < charArray.Length; i++) {
				var c = charArray[i];
				if (c != '0' &&
					c != '1') throw new InvalidDataException();

				Add(c != '0');
			}
		}

		public void Flush() {
			while (index > 0) Add(false);
		}

		public ReadOnlySpan<char> ToChars() {
			var output = new List<char>();
			foreach (var b in bytes) {
				var c = (char) b;
				output.Add(c);
			}

			return new ReadOnlySpan<char>(output.ToArray());
		}
	}
}