using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NondeterministicGrammarParser.parse;
using NondeterministicGrammarParser.parse.exceptions;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleParentIsCategory : Rule {

		private string category;
		private List<string> parentCategories;

		public RuleParentIsCategory(string category, params string[] parentCategories) :
			base(t => t.getAllChildrenOfCategory(category)) {
			this.category = category;
			this.parentCategories = parentCategories.ToList();
		}
		
		
		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			foreach (var parseNode in from node in nodes select node as CategoryNode) {
				if(parseNode == null) throw new NullReferenceException();
				if(!parseNode.category.name.Equals(category)) throw new IncorrectParseNodeCategoryException(parseNode.category.name, category);
				
				var parent = parseNode.parent;
				if (parent == null || !parentCategories.Contains((parent as CategoryNode)?.category.name)) return false;

			}

			return true;
		}
	}
}