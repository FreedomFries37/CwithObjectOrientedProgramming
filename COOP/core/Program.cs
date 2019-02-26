using System.Collections.Generic;
using System.Collections.ObjectModel;
using COOP.core.compiler.converters;
using COOP.core.compiler.COOP_file_to_COOP_objects;
using COOP.core.inheritence;
using COOP.core.structures;

namespace COOP.core {
	class Program {
		static void Main(string[] args) {


			if (args.Length > 0) {
				COOPClassParser parser =new COOPClassParser();
				for (var i = 0; i < args.Length; i++) {
					parser.parseFile(args[i])?.print();
				}
				
				return;
			}
			
			
			
			

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
						),
					new COOPFunction(
						"main",
						COOPPrimitives.integer,
						new Dictionary<string, COOPClass> {
							{"argv", COOPPrimitives.integer},
							{"argc", new COOPArray(COOPClass.String, 20)}
						}
					)
				},
				new Dictionary<string, COOPClass> {
					{"strings", new COOPArray(COOPClass.String, 15)}
				}
				);
			testClass.Functions["get"].Body = "\tString str = strings.get(index);\n\tstr.PrintLn();\n\treturn strings.get(index);";
			testClass.Functions["get"].AccessLevel = AccessLevel.Protected;
			testClass.imports.Add(new COOPArray(COOPClass.String, 4));

			testClass.Functions["main"].AccessLevel = AccessLevel.Public;
			testClass.Functions["main"].Body = "return 1;";
			
			
			var heiarchy = new ClassHierarchy();
			//heiarchy.addClass(COOPClass.String);
			heiarchy.print();
			testClass.addNonDefualtAccessLevel("strings", AccessLevel.Public);
			converter.convert(COOPClass.String, heiarchy);
			heiarchy.addClass(testClass);
			
			//heiarchy.createAllCFiles();
			
			/*
			foreach (FileConvertedInformation fileConvertedInformation in converter.convert(testClass, heiarchy)) {
				Console.WriteLine(fileConvertedInformation);
				StreamWriter w = File.CreateText(fileConvertedInformation.IntendedFileName);
				w.Write(fileConvertedInformation.FileContents);
				w.Flush();
				w.Close();
			}
			
			
			
			COOPFunctionConverter functionConverter = new COOPFunctionConverter(heiarchy, testClass);
			
			functionConverter.convert(testClass.Functions["get"]);
			
			COOPFileLexer fileLexer = new COOPFileLexer();
			fileLexer.loadFile("square.coop");
			foreach (string s in fileLexer.lex()) {
				Console.WriteLine(s);
			}
			
			*/
		}
	}
}