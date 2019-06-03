using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace Tests {
	[SetUpFixture]
	public class Setup {
		
		

		[OneTimeSetUp]
		public void Initialize() {
			var dir = Path.Join(TestContext.CurrentContext.TestDirectory, "resources");
			Directory.SetCurrentDirectory(dir);
		}
	}
}