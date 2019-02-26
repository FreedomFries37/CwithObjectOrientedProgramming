using System.Collections;
using System.Collections.Generic;
using System.IO;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core {

	public enum COOPProjectOutputLevel {
		C,
		Assembly,
		EXE
	}
	
	public class COOPProject {
		

		private ClassHierarchy hierarchy { get; }
		public string projectName { get; set; }
		private List<string> externalLibraries { get; }

		public COOPProjectOutputLevel outputLevel { get; set; } = COOPProjectOutputLevel.C;

		public string outputDir { get; set; }

		public static string cCompilerPath;

		public COOPProject(string projectName) {
			this.projectName = projectName;
			hierarchy = new ClassHierarchy();
			outputDir = Directory.GetCurrentDirectory();
		}

		public IEnumerator GetEnumerator() {
			return ((IEnumerable) externalLibraries).GetEnumerator();
		}

		public void Add(string item) {
			externalLibraries.Add(item);
		}

		public bool addClass(COOPClass coopClass) {
			return hierarchy.addClass(coopClass);
		}

		public bool contains(COOPClass coopClass) {
			return hierarchy.contains(coopClass);
		}

		public List<COOPClass> getLineage(COOPClass coopClass) {
			return hierarchy.getLineage(coopClass);
		}

		public COOPClass getParent(COOPClass coopClass) {
			return hierarchy.getParent(coopClass);
		}

		public COOPClass getClass(string className) {
			return hierarchy.getClass(className);
		}

		public void createAllCFiles() {
			createAllCFiles(outputDir);
		}

		private void createAllCFiles(string directory) {
			hierarchy.createAllCFiles(directory);
		}
	}
}