namespace COOP.core.compiler.COOP_objects_to_C_file {
	public interface FileCreator {

		/// <summary>
		/// creates files at a path
		/// </summary>
		/// <param name="outputPath">The intended destination of the files</param>
		/// <returns>number of files created</returns>
		int createFiles(string outputPath);
	}
}