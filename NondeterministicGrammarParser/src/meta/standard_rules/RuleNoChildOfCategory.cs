using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using NondeterministicGrammarParser.parse;
using NondeterministicGrammarParser.parse.exceptions;

namespace NondeterministicGrammarParser.meta.standard_rules {
	public class RuleChildOfCategory : Rule{
		private string category;
		private List<string> childCategories;

		public RuleChildOfCategory(string category, params string[] childCategories) :
		base(t => t.getAllChildrenOfCategory(category)){
			this.category = category;
			this.childCategories = childCategories.ToList();
		}
		

		internal override bool GetTruthValue(Collection<ParseNode> nodes) {
			foreach (var parseNode in from node in nodes select node as CategoryNode) {
				if(parseNode == null) throw new NullReferenceException();
				if(!parseNode.category.name.Equals(category)) throw new IncorrectParseNodeCategoryException(parseNode.category.name, category);
				
				foreach (var child in from f in parseNode.getChildren() where f is CategoryNode select (CategoryNode) f) {
					if (child.category.name.Equals(category)) return true;
				}
			}

			return false;
		}
	}
}