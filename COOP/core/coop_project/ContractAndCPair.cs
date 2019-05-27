using COOP.core.coop_project.file_types;

namespace COOP.core.coop_project {
	public class ContractAndCPair {

		public ContractFile contractFile { get; }
		public CFile cFile { get; }

		public ContractAndCPair(ContractFile contractFile, CFile cFile) {
			this.contractFile = contractFile;
			this.cFile = cFile;
		}
	}
}