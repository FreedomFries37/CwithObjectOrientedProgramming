using System.Collections;
using System.Collections.Generic;
using System.IO;
using COOP.core.structures.v2.global.type;
using COOP.core.structures.v2.global.type.hierarchy;

namespace COOP.core.coop_project {

	/*
	 * 
	 */
	
	
	public enum COOPProjectOutputLevel {
		C,
		Assembly,
		EXE
	}
	
	public class COOPProject {
		

		private AdvancedTypeHierarchy hierarchy { get; }
		public string projectName { get; set; }
		private List<string> externalLibraries { get; }
		private List<string> cpFiles { get; }

		public COOPProjectOutputLevel outputLevel { get; set; } = COOPProjectOutputLevel.C;

		public string outputDir { get; set; }

		public static string cCompilerPath;

		public COOPProject(string projectName) {
			this.projectName = projectName;
			hierarchy = new AdvancedTypeHierarchy();
			outputDir = Directory.GetCurrentDirectory();
			cpFiles = new List<string>();
		}

		public IEnumerator GetEnumerator() {
			return ((IEnumerable) externalLibraries).GetEnumerator();
		}

		public void AddExternalLibrary(string item) {
			externalLibraries.Add(item);
		}

		public void AddCPFile(string item) {
			cpFiles.Add(item);
		}

		public bool addClass(COOPType coopClass) {
			return hierarchy.add(coopClass);
		}

		public bool contains(COOPClass coopClass) {
			return hierarchy.contains(coopClass);
		}

		public COOPClass findNextAvailableParentClass(COOPAbstract a) {
			return hierarchy.findNextAvailableParentClass(a);
		}

		public CApplicableStructHierarchy convert() {
			return hierarchy.convert();
		}
	}
}