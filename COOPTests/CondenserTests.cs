using FileCondenser.core;
using NUnit.Framework;

namespace Tests {
	[TestFixture]
	public class CondenserTests {

		[Test]
		public void TestCondenser() {

			Condenser condenser = new Condenser("text.txt", "text2.txt");
			string condenseOutput;
			Assert.DoesNotThrow(() => condenseOutput = condenser.Condense());
			
			Assert.IsTrue(true);
		}
	}
}