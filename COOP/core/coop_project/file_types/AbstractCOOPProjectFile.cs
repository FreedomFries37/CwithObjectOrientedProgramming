using System.IO;

namespace COOP.core.coop_project {
	public abstract class AbstractCOOPProjectFile : ICOOPProjectFile{

		protected string intendedExtension { get; private set; }
		public string FilePath { private set; get; }

		public AbstractCOOPProjectFile(string intendedExtension, string filePath) {
			this.intendedExtension = intendedExtension;
			FilePath = filePath;
		}

		public string getPath() {
			return FilePath;
		}

		public bool isCorrectExtension() {
			return getExtension() == new FileInfo(FilePath).Extension;
		}

		public string getExtension() {
			return intendedExtension;
		}
	}
}