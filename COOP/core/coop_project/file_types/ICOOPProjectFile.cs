namespace COOP.core.coop_project.file_types {
	public interface ICOOPProjectFile {
		string getPath();

		string getExtension();

		bool isCorrectExtension();
	}
}