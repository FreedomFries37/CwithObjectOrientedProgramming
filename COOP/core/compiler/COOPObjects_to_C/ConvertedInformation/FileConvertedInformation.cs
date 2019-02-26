namespace COOP.core.compiler.converters.ConvertedInformation {
	public class FileConvertedInformation : ConvertedInformation{
		private string intendedFileName;
		private string fileContents;

		public bool hasMainMethod { get; set; }
		public string mainMethod { get; set; }

		public FileConvertedInformation(string intendedFileName, string fileContents) {
			this.intendedFileName = intendedFileName;
			this.fileContents = fileContents;
		}

		public string IntendedFileName => intendedFileName;

		public string FileContents => fileContents;

		public override string ToString() {
			return intendedFileName + "{\n" + fileContents + "}";
		}
	}
}