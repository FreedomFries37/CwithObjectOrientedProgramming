using System.Collections.Generic;

namespace COOP.core.structures.v2.global.type.hierarchy {
	public class CApplicableStructHierarchy {

		protected class Node {
			public COOPClass coopClass { get; }
			public Node parent { get; }
			public List<Node> children { get; }

			public Node(COOPClass coopClass, Node parent, List<Node> children) {
				this.coopClass = coopClass;
				this.parent = parent;
				this.children = children;
			}

			public Node(COOPClass coopClass, Node parent) {
				this.coopClass = coopClass;
				this.parent = parent;
				children = new List<Node>();
			}
		}

		private Node head;

		public CApplicableStructHierarchy(COOPClass head) {
			this.head = new Node(head, null);
		}

		public bool add(COOPClass coopClass, COOPClass parent) {
			var node = find(parent, head);
			if (node == null) return false;
			Node parentNode = node;
			parentNode.children.Add(new Node(coopClass, parentNode));
			return true;
		}

		
		private Node find(COOPClass coopClass, Node ptr) {
			if (ptr.coopClass == coopClass) return ptr;
			foreach (Node ptrChild in ptr.children) {
				var node = find(coopClass, ptrChild);
				if (node != null) return node;
			}

			return null;
		}
	}
}