using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileCondenser.core;
using NUnit.Framework;

namespace Tests {
	[TestFixture]
	public class HuffmanTreeTests {

		
		
		
		[Test]
		public void ReadFileTests() {
			
			Assert.DoesNotThrow(
				() => File.ReadAllText(
					"text.txt")
			);
			
			Assert.DoesNotThrow(
				() => File.ReadAllText(
					"text2.txt")
			);
		}
		
		
		
		

		[Test(Description = "Test For Successful Construction of Simple Tree From File")]
		public void TestSimpleHuffmanTreeFromFile() {
			var allText = File.ReadAllText("text.txt");
			var allFromString = CharCountDeterminer.GetAllFromString(allText);
			
			Assert.NotNull(HuffmanTree.AssembleTree(allFromString));
		}

		[Test]
		public void TestCharCountDeterminer() {
			var dict = NumberCharacterGenerator(100);
			string w = GenerateString(dict);

			var allFromString = CharCountDeterminer.GetAllFromString(w);
			Assert.IsTrue(Equals(dict, allFromString));
			
			dict = NumberCharacterGenerator(1000);
			w = GenerateString(dict);

			allFromString = CharCountDeterminer.GetAllFromString(w);
			Assert.IsTrue(Equals(dict, allFromString));
			
			dict = NumberCharacterGenerator(0);
			w = GenerateString(dict);

			allFromString = CharCountDeterminer.GetAllFromString(w);
			Assert.IsTrue(Equals(dict, allFromString));
			
		}

		private static bool Equals(Dictionary<char, long> a, Dictionary<char, long> b) {
			var except = a.Except(b).Concat(b.Except(a));
			foreach (var keyValuePair in except) {
				if (keyValuePair.Value > 0)
					return false;
			}
			
			return true;
		}

		private static Dictionary<char, long>
			NumberCharacterGenerator(int max, char minChar = ' ', char maxChar = '~') =>
			NumberCharacterGenerator(0, max, minChar, maxChar);
		private static Dictionary<char, long> NumberCharacterGenerator(int min, int max, char minChar = ' ', char maxChar = '~') {
			var output = new Dictionary<char, long>();
			
			for (char c = minChar; c <= maxChar; c++) {
				int num = new Random().Next(min, max);
				output.Add(c, num);
			}

			return output;
		}

		private static long Total(Dictionary<char, long> dict) {
			long sum = 0;
			foreach (long dictValue in dict.Values) {
				sum += dictValue;
			}

			return sum;
		}

		private static string GenerateString(Dictionary<char, long> numChars) {
			var copy = new Dictionary<char, long>(from kvp in numChars where kvp.Value > 0 select kvp);
			StringBuilder output = new StringBuilder();

			long total = Total(numChars);
			
			
			while (copy.Keys.Count > 0) {
				
				Random rand = new Random();
				List<char> keys = copy.Keys.ToList();
				char rKey = keys[rand.Next(keys.Count)];

				output.Append(rKey);

				if (--copy[rKey] == 0) {
					copy.Remove(rKey);
				}
				
			}


			return output.ToString();
		}
	}
}