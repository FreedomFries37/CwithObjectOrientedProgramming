using System.IO;
using System.Linq;
using FileCondenser.core;
using FileCondenser.core.output;
using NUnit.Framework;

namespace COOPTests.CondenserTests {
	[TestFixture]
	public class CondenserInputOutputTests {
		[Test]
		[TestCase(5, 1, 'a', 'c')]
		[TestCase(100, 1, 'a', 'z')]
		[TestCase(25, 25, 'a', 'z')]
		public void ChainReproductability(int maxChars, int minChars = 0, char minChar = ' ', char maxChar = '~') {
			var dict = HuffmanTreeTests.NumberCharacterGenerator(minChars, maxChars, minChar, maxChar);
			var w = HuffmanTreeTests.GenerateString(dict);
			var chain = new HuffmanChain(w);


			var output = chain.ToString();
			TestContext.WriteLine("Original: " + output);
			var recreated = HuffmanChain.Reconstruct(output);

			var recreatedString = chain.ToString();
			TestContext.WriteLine("Recreated: " + recreatedString);

			Assert.AreEqual(output, recreatedString);
			Assert.IsTrue(chain.Equals(recreated));
		}


		[Test]
		[TestCase("text.txt")]
		[TestCase("text2.txt")]
		public void ChainReproductabilityFromFile(string path) {
			var w = File.ReadAllText(path);
			var chain = new HuffmanChain(w);

			var chainOutputService = new HuffmanChain.HuffmanChainOutputService();
			var chainInputService = new HuffmanChain.HuffmanChainInputService();

			var output = chainOutputService.CreateOutput(chain);
			TestContext.WriteLine("Original: " + output);
			var recreated = chainInputService.CreateFromInput(output);

			var recreatedString = chainOutputService.CreateOutput(recreated);
			TestContext.WriteLine("Recreated: " + recreatedString);

			Assert.AreEqual(output, recreatedString);
			Assert.IsTrue(chain.Equals(recreated));
		}

		[Test]
		[TestCase("text.txt", ExpectedResult = true)]
		[TestCase("text2.txt", ExpectedResult = true)]
		public bool CondensedFileSmallerThanOriginal(string path) {
			var w = File.ReadAllText(path);
			var length = w.Length;

			var manager = new FileCondenserManager();
			manager.AddFile(path);

			manager.OutputFileName = nameof(CondensedFileSmallerThanOriginal) + path;

			Assert.DoesNotThrow(() => { manager.CreateFile(); });

			var fc = File.ReadAllText(manager.OutputFileName);
			int? fcLength = fc.Length;
			Assert.IsTrue(fcLength.HasValue);

			TestContext.WriteLine($"Created File Size: {fcLength}\tOriginal File Size: {length}");

			return fcLength < length;
		}

		[Test]
		[TestCase("text.txt")]
		[TestCase("text.txt", "text2.txt")]
		public void CreateCondensedFileInputFilesOnly(params string[] paths) {
			var manager = new FileCondenserManager();
			foreach (var path in paths) manager.AddFile(path);

			manager.OutputFileName =
				manager.OutputFileName + string.Join("_", from f in paths select new FileInfo(f).Name);

			Assert.DoesNotThrow(() => { manager.CreateFile(); });
		}
	}
}