namespace COOP.core.coop_project {
	public interface ICOOPProjectFile {
		string getPath();

		string getExtension();

		bool isCorrectExtension();
	}
}