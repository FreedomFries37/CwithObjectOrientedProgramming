namespace COOP.core.compiler {
	public class FileConvertedInformation : ConvertedInformation{
		private string intendedFileName;
		private string fileContents;

		public FileConvertedInformation(string intendedFileName, string fileContents) {
			this.intendedFileName = intendedFileName;
			this.fileContents = fileContents;
		}

		public string IntendedFileName => intendedFileName;

		public string FileContents => fileContents;
	}
}