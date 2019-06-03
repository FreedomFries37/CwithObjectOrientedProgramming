using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileCondenser.core;
using NUnit.Framework;

namespace COOPTests.CondenserTests {
	[TestFixture]
	public class HuffmanTreeTests {
		internal static bool Equals(Dictionary<char, long> a, Dictionary<char, long> b) {
			var except = a.Except(b).Concat(b.Except(a));
			foreach (var keyValuePair in except)
				if (keyValuePair.Value > 0)
					return false;

			return true;
		}

		internal static Dictionary<char, long>
			NumberCharacterGenerator(int max, char minChar = ' ', char maxChar = '~') {
			return NumberCharacterGenerator(0, max, minChar, maxChar);
		}

		internal static Dictionary<char, long> NumberCharacterGenerator(int min, int max, char minChar = ' ',
																		char maxChar = '~') {
			var output = new Dictionary<char, long>();

			for (var c = minChar; c <= maxChar; c++) {
				var num = new Random().Next(min, max);
				output.Add(c, num);
			}

			return output;
		}

		internal static long Total(Dictionary<char, long> dict) {
			long sum = 0;
			foreach (var dictValue in dict.Values) sum += dictValue;

			return sum;
		}

		internal static string GenerateString(Dictionary<char, long> numChars) {
			var copy = new Dictionary<char, long>(from kvp in numChars where kvp.Value > 0 select kvp);
			var output = new StringBuilder();

			var total = Total(numChars);


			while (copy.Keys.Count > 0) {
				var rand = new Random();
				var keys = copy.Keys.ToList();
				var rKey = keys[rand.Next(keys.Count)];

				var index = rand.Next(output.Length);
				output.Insert(index, rKey);

				if (--copy[rKey] == 0) copy.Remove(rKey);
			}


			return output.ToString();
		}


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

		[Test]
		public void TestCharCountDeterminer() {
			var dict = NumberCharacterGenerator(100);
			var w = GenerateString(dict);

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


		[Test(Description = "Test For Successful Construction of Simple Tree From File")]
		public void TestSimpleHuffmanTreeFromFile() {
			var allText = File.ReadAllText("text.txt");
			var allFromString = CharCountDeterminer.GetAllFromString(allText);

			Assert.NotNull(HuffmanTree.AssembleTree(allFromString));
		}
	}
}