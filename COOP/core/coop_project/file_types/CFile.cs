namespace COOP.core.coop_project {
	
	/// <summary>
	/// a C file, where no translation is done.
	/// Must be paired with a contract file for it to be used in a COOP Project
	/// </summary>
	public class CFile : AbstractCOOPProjectFile{
		public CFile(string filePath) : base(".c", filePath) { }
	}
}