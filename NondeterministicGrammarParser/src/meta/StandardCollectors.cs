using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using NondeterministicGrammarParser.parse;

namespace NondeterministicGrammarParser.meta {
	public class StandardCollectors {

		private class CategoryCollectorWrapper : NodeCollectorWrapper {
			protected string category;

			public CategoryCollectorWrapper(string category) {
				this.category = category;
			}

			public virtual NodeCollector GetCollector() {
				return tree => tree.getAllChildrenOfCategory(category);
			}
		}
		
		private class CategoryCollectorOfParentWrapper : CategoryCollectorWrapper {
			private string parentCategory;

			public CategoryCollectorOfParentWrapper(string category, string parentCategory) : base(category) {
				this.parentCategory = parentCategory;
			}


			public override NodeCollector GetCollector() {
				return tree => new Collection<ParseNode>((from f in tree.getAllChildrenOfCategory(category) 
					where (f.parent as CategoryNode)?.category.name.Equals(parentCategory) ?? false 
					select f).ToList());
			}
		}
		
		private class CategoryCollectorMultiWrapper : CategoryCollectorWrapper {
			private NodeCollector parent { get; }

			public CategoryCollectorMultiWrapper(string s1, string s2, params string[] s) : base(s1) {
				if (s.Length == 0) {
					parent = CategoryCollector(s2);
				} else {
					var list = new List<string>(s);
					var s3 = s[0];
					list.RemoveAt(0);
					parent = new CategoryCollectorMultiWrapper(s2, s3, list.ToArray()).GetCollector();
				}
			}


			public override NodeCollector GetCollector() {
				if (parent == null) return base.GetCollector(); 
				return tree => {
					var parseNodes = parent(tree);
					var collection = new Collection<ParseNode>();
					foreach (ParseNode parseNode in parseNodes) {
						var pTree = new ParseTree(parseNode);
						var nodes = base.GetCollector()(pTree);
						foreach (ParseNode node in nodes) {
							collection.Add(node);
						}
					}

					return collection;
				};
			}
		}

		public static NodeCollector CategoryCollector(string cat) {
			return new CategoryCollectorWrapper(cat).GetCollector();
		}
		
		public static NodeCollector CategoryOfParentCollector(string cat, string parent) {
			return new CategoryCollectorOfParentWrapper(cat, parent).GetCollector();
		}
		
		public static NodeCollector CategoryCollectorMulti(string cat, string s1, params string[] s) {
			return new CategoryCollectorMultiWrapper(cat, s1, s).GetCollector();
		}
	}
}