using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using COOP.core;
using COOP.core.compiler;
using COOP.core.compiler.converters;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP {
	class Program {
		static void Main(string[] args) {
			
			
			
			
			
			COOPClassConverter converter = new COOPClassConverter();
			
			COOPClass testClass = new COOPClass(
				"testClass",
				COOPClass.Base,
				new Collection<COOPFunction> {
					new COOPFunction(
						"get",
						COOPClass.String,
						new Dictionary<string, COOPClass> {
							{"index", COOPPrimitives.integer}
						}
						)
				},
				new Dictionary<string, COOPClass> {
					{"strings", new COOPArray(COOPClass.String, 15)}
				}
				);
			testClass.Functions["get"].Body = "\treturn strings.get(index);";
			
			
			
			var heiarchy = new ClassHierarchy();
			//heiarchy.addClass(COOPClass.String);
			heiarchy.print();
			testClass.addNonDefualtAccessLevel("strings", AccessLevel.Public);
			converter.convert(COOPClass.String, heiarchy);
			heiarchy.addClass(testClass);
			converter.convert(testClass, heiarchy);
			
			
			COOPFunctionConverter functionConverter = new COOPFunctionConverter(heiarchy, testClass);
			
			functionConverter.convert(testClass.Functions["get"]);
			
			COOPFileLexer fileLexer = new COOPFileLexer();
			fileLexer.loadFile("square.coop");
			foreach (string s in fileLexer.lex()) {
				Console.WriteLine(s);
			}
		}
	}
}