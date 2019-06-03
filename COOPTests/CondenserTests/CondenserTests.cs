using System.IO;
using FileCondenser.core;
using FileCondenser.core.expand;
using NUnit.Framework;

namespace COOPTests.CondenserTests {
	[TestFixture]
	public class CondenserTests {
		private const int ATTEMPTS = 25;

		[Test]
		[Repeat(ATTEMPTS)]
		public void ChainEfficiency() {
			var dict = HuffmanTreeTests.NumberCharacterGenerator(200);
			var w = HuffmanTreeTests.GenerateString(dict);

			HuffmanChain chain;
			HuffmanTree tree;
			string cW, cT;

			var condenser = new SingleCondenser();
			(cW, chain) = condenser.Condense(w);
			(cT, tree) = condenser.CondenseToTree(w);

			Assert.NotNull(chain);
			Assert.NotNull(tree);

			Assert.LessOrEqual(cW.Length, cT.Length);
			Assert.LessOrEqual((double) cW.Length / cT.Length, .95);


			TestContext.WriteLine(
				$"Chain vs Tree Efficiency: cW/cT = {(double) cW.Length / cT.Length:P} size decrease");
			TestContext.WriteLine($"Chain Overall Efficiency: cW/w = {(double) cW.Length / w.Length:P} size decrease");
			TestContext.WriteLine($"Tree Overall Efficiency: cW/w = {(double) cT.Length / w.Length:P} size decrease");
			TestContext.WriteLine();
		}

		[Test]
		[TestCase(5, 1, 'a', 'c')]
		public void ChainStringService(int maxChars, int minChars = 0, char minChar = ' ', char maxChar = '~') {
			var dict = HuffmanTreeTests.NumberCharacterGenerator(minChars, maxChars, minChar, maxChar);
			var w = HuffmanTreeTests.GenerateString(dict);

			HuffmanChain chain;
			var condenser = new SingleCondenser();
			chain = condenser.Condense(w).Item2;

			var output = new HuffmanChain.HuffmanChainOutputService().CreateOutput(chain);
			TestContext.WriteLine(output);
		}

		[Test]
		[TestCase("text.txt")]
		[TestCase("text2.txt")]
		public void FileCondense(string path) {
			var w = File.ReadAllText(path);
			HuffmanChain chain;
			HuffmanTree tree;
			string cW, cT;

			var condenser = new SingleCondenser();
			(cW, chain) = condenser.Condense(w);
			(cT, tree) = condenser.CondenseToTree(w);

			Assert.NotNull(chain);
			Assert.NotNull(tree);

			Assert.LessOrEqual(cW.Length, w.Length);
			Assert.LessOrEqual(cT.Length, w.Length);
			Assert.LessOrEqual(cW.Length, cT.Length);
			Assert.LessOrEqual((double) cW.Length / cT.Length, .95);

			var expander = new SingleExpander();
			var expanded = expander.Expand(cW, chain, w.Length);

			TestContext.WriteLine(
				$"Chain vs Tree Efficiency: cW/cT = {(double) cW.Length / cT.Length:P} size decrease");
			TestContext.WriteLine($"Chain Overall Efficiency: cW/w = {(double) cW.Length / w.Length:P} size decrease");
			TestContext.WriteLine($"Tree Overall Efficiency: cW/w = {(double) cT.Length / w.Length:P} size decrease");

			var output = new HuffmanChain.HuffmanChainOutputService().CreateOutput(chain);
			TestContext.WriteLine(output);

			Assert.AreEqual(w, expanded);
		}


		[Test]
		[TestCase(0)]
		[TestCase(50)]
		[TestCase(100)]
		[TestCase(150)]
		[TestCase(200)]
		[TestCase(50, 1, 'a', 'z')]
		[TestCase(100, 1, 'a', 'z')]
		[TestCase(25, 25, 'a', 'z')]
		public void TestCondenser(int maxChars, int minChars = 0, char minChar = ' ', char maxChar = '~') {
			var dict = HuffmanTreeTests.NumberCharacterGenerator(minChars, maxChars, minChar, maxChar);
			var w = HuffmanTreeTests.GenerateString(dict);
			var total = HuffmanTreeTests.Total(dict);

			var condenser = new SingleCondenser();
			var condenseOutput = "";
			HuffmanChain chain = null;

			Assert.DoesNotThrow(() => {
				(string w, HuffmanChain c) valueTuple = condenser.Condense(w);
				condenseOutput = valueTuple.w;
				chain = valueTuple.c;
			});


			Assert.LessOrEqual(condenseOutput.Length, w.Length);

			Assert.NotNull(chain);

			var expander = new SingleExpander();
			var expanded = expander.Expand(condenseOutput, chain, total);


			Assert.AreEqual(w, expanded);

			TestContext.WriteLine(
				$"Chain Overall Efficiency: cW/w = {(double) condenseOutput.Length / w.Length:P} size decrease");
			TestContext.WriteLine();
		}
	}
}