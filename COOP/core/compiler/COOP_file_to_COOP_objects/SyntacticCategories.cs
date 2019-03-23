using System.Runtime.CompilerServices;

using NondeterminateGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public class SyntacticCategories {

	
		public static Category symbolStartChar { get; } = new Category("symbol_start_char") {new Terminal('_') };
		public static Category symbolCont { get; } = new Category("symbol_cont");
		
		public static Category symbol { get;} = new Category("symbol") {{symbolStartChar, symbolCont}};
		public static Category symbolContTail { get; }  = new Category("symbol_cont_tail") {symbolCont};
		public static Category symbolContChar { get; } = new Category("symbol_cont_char") {new Terminal('_')};
		
		public static Category stringEscapeChar { get; } = 
			new Category("string_escape_char") {
			new Terminal('n'),
			new Terminal('t'),
			new Terminal('r'),
			new Terminal('0'),
			new Terminal('v'),
			new Terminal('"'),
			new Terminal('/')
		};

		public static Category stringtail { get; } = new Category("string_tail");
		public static Category @string { get; } = new Category("string") {{new Terminal('"'), stringtail, new Terminal('"')}};
		public static Category stringchar { get; } = new Category("string_char") {{new Terminal('\\'), stringEscapeChar}};
		
		public static Category digit { get; } = new Category("digit");
		
		public static Category integerTail { get; } = new Category("integer_tail", true);
		public static Category @integer { get; } = new Category("integer") {{digit, integerTail}};

		public static Category floatingPoint { get; } = new Category("floating_point") {{integer, ".", integer}};
		
		public static Category @float { get; } = new Category("float") {{floatingPoint, "l"}, {floatingPoint, "L"}};
		public static Category @double { get; } = new Category("double") {{floatingPoint, "d"}, {floatingPoint, "D"}};

		public static Category number { get; } = new Category("number") {integer, @float, @double};

		public static Category literal { get; } = new Category("literal") {number, @string};

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
		
		
		public static Category objectFactor{ get; } = new Category("object_factor") {
			{"(", objectExpression, ")"},
			objectAccess,
			literal
		};
		


	
		public static Category COOPClassFile { get; } = new Category("COOP_class_file");

		
		public static Category import { get; } = new Category("import") {{"using", symbol, ";"}};
		public static Category importTail { get; } = new Category("import_tail");
		public static Category importList { get; } = new Category("import_list") {{import, importTail}};

		public static Category classDefinition { get; } = new Category("class");
		public static Category classInformation{ get; } = new Category("class_info");

		public static Category classNameSub { get; } = new Category("class_name_sub");
		public static Category className { get; } = new Category("class_name") {{symbol, classNameSub}};
		public static Category parentClassName { get; } = new Category("parent_class") {{symbol, classNameSub}};

		
		
		public static Category accessLevel { get; } = new Category("access_level", true) {"public", "private", "protected"};
		
		public static Category fieldDeclaration { get; } = new Category("field_dec") {{accessLevel, varDeclaration, ";"}};
		
		public static Category parameterList { get; } = new Category("parameters_list", true);
		public static Category parameterTail { get; } = new Category("parameters_tail") {",", parameterList};
		public static Category parameters { get; } = new Category("parameters") {{"(", parameterList, ")"}};

		
		public static Category contstructorDeclaration { get; } = new Category("constructor_dec") {{accessLevel, className, parameters}};

		public static Category assignment { get; } = new Category("assignment") {{"=", objectExpression}};
		public static Category varDeclaration { get; } = new Category("var_dec") {{className, symbol}, {className, symbol, assignment}};
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

		static SyntacticCategories() {


			symbolContChar.addTerminalRulesForRange('a', 'z');
			symbolContChar.addTerminalRulesForRange('A', 'A');
			symbolContChar.addTerminalRulesForRange('0', 'Z');



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
	}

		public Category start { get; } = COOPClassFile;
		
		

	}
}