using System.IO;
using NUnit.Framework;

namespace COOPTests.CondenserTests {
	[SetUpFixture]
	public class Setup {
		[OneTimeSetUp]
		public void Initialize() {
			var dir = Path.Join(TestContext.CurrentContext.TestDirectory, "resources");
			Directory.SetCurrentDirectory(dir);
		}
	}
}