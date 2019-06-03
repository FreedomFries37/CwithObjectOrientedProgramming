using System.Runtime.CompilerServices;
using NondeterministicGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	/// <summary>
	/// All of the Syntactic Categories for the COOP files
	/// </summary>
	public partial class SyntacticCategories {




		public static Category objectMemberAccess { get; } =
			new Category("object_member_access"); //guaranteed to end in member

		public static Category objectFunction { get; } =
			new Category("object_function_access"); //guaranteed to end in function

		public static Category objectCast { get; } = new Category("object_cast");

		public static Category booleanExpression { get; } = new Category("boolean_expression");

		public static Category objectAccess { get; } =
			new Category("object_access") {objectMemberAccess, objectFunction, objectCast}; //ends in either

		public static Category objectExpression { get; } = new Category("object_expression");

		public static Category objectTail { get; } = new Category("object_expression_tail", true) {
			{"+", objectExpression},
			{"-", objectExpression},
			{"|", objectExpression},
			{"||", objectExpression}
		};

		public static Category objectGroup { get; } = new Category("object_group");

		public static Category objectGroupTail { get; } = new Category("object_group_tail", true) {
			{"*", objectGroup},
			{"/", objectGroup},
			{"&", objectGroup},
			{"&&", objectGroup},
			{"^", objectGroup}
		};


		public static Category objectFactor { get; }

		



		
		public static Category assignment { get; } = new Category("assignment") {{"=", objectExpression}};
		public static Category varDeclaration { get; } = new Category("var_dec");
		public static Category varDeclarationStrict { get; } = new Category("var_dec_strict");

		public static Category parameterList { get; } = new Category("parameters_list");
		public static Category parameterTail { get; } = new Category("parameters_tail", true) {{",", parameterList}};
		public static Category parameters { get; } = new Category("parameters") {{"(", parameterList, ")"}, {'(', ')'}};
		


		public static Category functionSign { get; } = new Category("function_sign");

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
		public static Category block { get; } = new Category("block") {{'{', listStatement, '}'}};

		
	
		
		public static Category start { get; }
		
		static SyntacticCategories() {
			
			objectFactor = new Category("object_factor") {
				{"(", objectExpression, ")"},
				objectAccess,
				literal
			};

			symbol.StrictTokenUsage = true;
			integer.StrictTokenUsage = true;
			accessLevel.StrictTokenUsage = true;

			symbolContChar.addTerminalRulesForRange('a', 'z');
			symbolContChar.addTerminalRulesForRange('A', 'A');
			symbolContChar.addTerminalRulesForRange('0', 'Z');

			varDeclaration.Add(className, symbol);
			varDeclarationStrict.Add(className, symbol);
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

			COOPClassFile.Add(importList, classDefinition);
			
			importList.Add(import, importList);
			import.Add("using", className);
			
			className.Add(symbol, classNameTail);
			classNameTail.Add(".", className);
			
			classDefinition.Add("class", symbol, parentClass, '{', classBodyList, '}');
			
			parentClass.Add("(", className, ")");
			
			classBodyList.Add(classBodyPart, classBodyList);
			classBodyPart.Add(accessLevel, className, bodyPartTail);
			
			bodyPartTail.Add(constructor);
			bodyPartTail.Add(symbolDeclaration);
			
			constructor.Add(parameters, block);
			
			symbolDeclaration.Add(symbol, symbolDeclarationTail);
			symbolDeclarationTail.Add(';');
			symbolDeclarationTail.Add(assignment, ';');
			symbolDeclarationTail.Add(functionDeclaration);
			
			functionDeclaration.Add(parameters, block);

			
			
			start = COOPClassFile;
			if(start.validate()) start.print();
		}

		
		
		

	}
}