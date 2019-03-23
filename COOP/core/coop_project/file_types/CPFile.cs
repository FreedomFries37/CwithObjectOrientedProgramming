namespace COOP.core.coop_project {
	
	/// <summary>
	/// One step in between true C and COOP files
	/// Use COOP syntax, and is translated into a .c and .h files
	/// </summary>
	public class CPFile : AbstractCOOPProjectFile{
		public CPFile(string filePath) : base("cp", filePath) { }
	}
}