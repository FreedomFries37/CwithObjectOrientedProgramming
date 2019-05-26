using System.Runtime.CompilerServices;

using NondeterminateGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	/// <summary>
	/// All of the Syntactic Categories for the COOP files
	/// </summary>
	public partial class SyntacticCategories {

	
		

		public static Category objectMemberAccess { get; } = new Category("object_member_access");//guaranteed to end in member
		
		public static Category objectFunction { get; }  = new Category("object_function_access");//guaranteed to end in function
		
		public static Category objectCast { get; } = new Category("object_cast");

		public static Category booleanExpression { get; } = new Category("boolean_expression");
		public static Category objectAccess { get; } = new Category("object_access") {objectMemberAccess, objectFunction, objectCast}; //ends in either

		public static Category objectExpression { get; } = new Category("object_expression");
		public static Category objectTail { get; } = new Category("object_expression_tail", true) {
			{"+", objectExpression},
			{"-", objectExpression},
			{"|", objectExpression},
			{"||", objectExpression}
		};
		
		public static Category objectGroup{ get; } = new Category("object_group");
		public static Category objectGroupTail { get; } = new Category("object_group_tail", true) {
			{"*", objectGroup},
			{"/", objectGroup},
			{"&", objectGroup},
			{"&&", objectGroup},
			{"^", objectGroup}
		};
		
		
		public static Category objectFactor{ get; } 
		
		public static Category COOPClassFile { get; } = new Category("COOP_class_file");

		
		public static Category import { get; } = new Category("import");
		public static Category importTail { get; } = new Category("import_tail");
		public static Category importList { get; } = new Category("import_list") {{import, importTail}};

		public static Category classDefinition { get; } = new Category("class");
		public static Category classInformation{ get; } = new Category("class_info");

		public static Category classNameSub { get; } = new Category("class_name_sub");
		public static Category className { get; } = new Category("class_name");
		public static Category parentClassName { get; }

		
		
		public static Category accessLevel { get; } = new Category("access_level", true) {"public", "private", "protected"};
		
		public static Category assignment { get; } = new Category("assignment") {{"=", objectExpression}};
		public static Category varDeclaration { get; } = new Category("var_dec");
		public static Category fieldDeclaration { get; } = new Category("field_dec") {{accessLevel, varDeclaration, ";"}};
		public static Category fieldList { get; } = new Category("field_list", true);
		
		public static Category parameterList { get; } = new Category("parameters_list", true);
		public static Category parameterTail { get; } = new Category("parameters_tail") {",", parameterList};
		public static Category parameters { get; } = new Category("parameters") {{"(", parameterList, ")"}};

		
		public static Category contstructorDeclaration { get; } = new Category("constructor_dec") {{accessLevel, className, parameters}};

	
		
		public static Category functionSign { get; } = new Category("function_sign") {{symbol, parameters}};

		public static Category executableState { get; } = new Category("executable_statement") {
			functionSign,
			objectFunction,
			varDeclaration,
			{varDeclaration, assignment}
		};

		public static Category conditional { get; } = new Category("conditional", true);

		public static Category statement { get; } = new Category("statement") {{conditional, executableState, ";"}};

		
		public static Category listStatement { get; } = new Category("list_statement", true);
		public static Category listStatementtail { get; } = new Category("list_statement_tail") {listStatement};
		public static Category block { get; } = new Category("block");

		public static Category functionDeclaration { get; } = new Category("function") {{accessLevel, functionSign, block}};

		
		public static Category start { get; } = COOPClassFile;
		static SyntacticCategories() {
			
			objectFactor = new Category("object_factor") {
				{"(", objectExpression, ")"},
				objectAccess,
				literal
			};

			symbol.StrictTokenUsage = true;
			integer.StrictTokenUsage = true;
			accessLevel.StrictTokenUsage = true;
			
			import.Add("using", symbol, ";");

			className.Add(symbol, classNameSub);
			parentClassName = new Category("parent_class") {{symbol, classNameSub}};

			symbolContChar.addTerminalRulesForRange('a', 'z');
			symbolContChar.addTerminalRulesForRange('A', 'A');
			symbolContChar.addTerminalRulesForRange('0', 'Z');

			varDeclaration.Add(className, symbol);
			varDeclaration.Add(className, symbol, assignment);

			symbolCont.Add(symbolContChar, symbolContTail);
			symbolCont.Add();

			symbolStartChar.addTerminalRulesForRange('a', 'z');
			symbolStartChar.addTerminalRulesForRange('A', 'Z');

			digit.addTerminalRulesForRange('0', '9');
			integerTail.Add(integer);

			stringchar.addTerminalRulesForRange(' ', '!');
			stringchar.addTerminalRulesForRange('#', '[');
			stringchar.addTerminalRulesForRange(']', '~');



			stringtail.Add();
			stringtail.Add(stringchar, stringtail);


			importTail.Add();
			importTail.Add(import, importList);

			COOPClassFile.Add(importList, classDefinition);
			COOPClassFile.Add(classDefinition);

			classNameSub.Add();
			classNameSub.Add(".", className);

			classDefinition.Add("class", className, "[", parentClassName, "]", "{", classInformation, "}");
			classDefinition.Add("class", className, "{", classInformation, "}");

			parameterList.Add(varDeclaration, parameterTail);

			objectFunction.Add(objectAccess, ".", functionSign);
			objectMemberAccess.Add(objectAccess, ".", symbol);

			objectFactor.Add("!", objectFactor);
			statement.Add(conditional, block);
			
			listStatement.Add(statement, listStatementtail);

			block.Add("{", listStatement, "}");
			
			
			classInformation.Add(fieldList);
			fieldList.Add(fieldDeclaration, fieldList);


			if(start.validate()) start.print();
		}

		
		
		

	}
}