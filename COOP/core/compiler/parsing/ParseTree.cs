﻿using System.Collections.Generic;

namespace COOP.core.compiler.parsing {
	public class ParseTree {
		private ParseNode head;
		public readonly bool successfulParse;

		public ParseNode Head => head;

		public int Count => head != null ? head.Count() : 0;

		public ParseTree(ParserFunction func, bool cleanup = true) {
			successfulParse = func(out head);
			if(successfulParse && cleanup) head.CleanUp();
		}

		public void PrintTree() {
			head.Print(0);
		}
		public void PrintTree(int maxDepth) {
			head.Print(0,maxDepth);
		}


		public ParseNode this[string s] => head[s];

		public ParseNode this[int i] => head[i];

		public List<ParseNode> GetAllOfType(string type) {
			return head.GetAllOfType(type);
		}

		public List<ParseNode> GetAllOfTypeOnlyDirectChildren(string s) {
			return head.GetAllOfTypeOnlyDirectChildren(s);
		}
	}
}