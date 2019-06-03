using System;
using System.Collections.Generic;
using System.IO;

namespace FileCondenser {
	public class BitStream {
		private byte currentByte = 0;
		private int index = 0;
		private List<byte> bytes;

		public BitStream() {
			bytes = new List<byte>();
		}

		public void Add(bool b) {
			byte f = currentByte;
			byte _b = (byte) (b ? 0b1 : 0b0);

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
			char[] charArray = s.ToCharArray();
			for (var i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if(c != '0' && c != '1') throw new InvalidDataException();

				Add(c != '0');
				
			}
		}

		public void Flush() {
			while (index > 0) {
				Add(false);
			}
		}

		public ReadOnlySpan<char> ToChars() {
			var output = new List<char>();
			foreach (var b in bytes) {
				char c = (char) b;
				output.Add(c);
			}

			return new ReadOnlySpan<char>(output.ToArray());
		}
	}
}