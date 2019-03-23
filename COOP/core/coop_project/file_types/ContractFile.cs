namespace COOP.core.coop_project {
	
	/// <summary>
	/// The contract file that must be paired with a c file. Converts to a .h file
	/// </summary>
	public class ContractFile : AbstractCOOPProjectFile{
		public ContractFile(string filePath) : base(".contract", filePath) { }
	}
}