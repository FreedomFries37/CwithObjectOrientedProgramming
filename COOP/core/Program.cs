using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using COOP.core.compiler.COOP_file_to_COOP_objects;
using COOP.core.coop_project;
using COOP.core.structures;
using COOP.core.structures.v2.global.modifiers;
using COOP.core.structures.v2.global.type.included;
using global::COOP.core.structures.v2.global;

namespace COOP.core {
	class Program {
		static void Main(string[] args) {
			COOPProject project = new COOPProject("Test");
			project.addClass(IncludedClasses.String);
		}
	


	}
	
	
}