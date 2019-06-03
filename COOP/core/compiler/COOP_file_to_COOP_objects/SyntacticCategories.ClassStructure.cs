using NondeterministicGrammarParser.parse.syntactic;

namespace COOP.core.compiler.COOP_file_to_COOP_objects {
	public partial class SyntacticCategories {
		public static Category COOPClassFile { get; } = new Category("COOP_class_file");


		public static Category import { get; } = new Category("import");
		public static Category importList { get; } = new Category("import_list", true);

		public static Category classDefinition { get; } = new Category("class");
		

		public static Category classNameSub { get; } = new Category("class_name_sub");
		public static Category className { get; } = new Category("class_name");
		public static Category classNameTail { get; } = new Category("class_name_tail", true);
		public static Category parentClass { get; } = new Category("parent_class", true);
		
		public static Category classBodyList { get; } = new Category("class_body_list", true);

		public static Category classBodyPart { get; } = new Category("class_body_part");
		public static Category bodyPartTail { get; } = new Category("body_part_tail");
		
		
		public static Category accessLevel { get; } = new Category("access_level", true)
			{"public", "private", "protected"};

		
		public static Category symbolDeclaration { get; } = new Category("symbol_dec");
		public static Category symbolDeclarationTail { get; } = new Category("symbol_dec_tail");
			

		public static Category constructor { get; } = new Category("constructor");

		public static Category functionDeclaration { get; } = new Category("function_dec");

	}
}