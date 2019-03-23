namespace COOP.core.coop_project {
	
	/// <summary>
	/// Standard COOP class file, directly translates into 1 or 2 .h files and 1 .c file
	/// </summary>
	public class COOPClassFile : AbstractCOOPProjectFile{
		public COOPClassFile(string filePath) : base("coop", filePath) { }
	}
}