using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using COOP.core.structures;

namespace COOP.core.inheritence {
	public class ClassHierarchy {

		private class Node {
			COOPClass @class;
			List<Node> children;
			Node parent;

			public Node(COOPClass @class, Node parent) {
				this.@class = @class;
				children = new List<Node>();
				this.parent = parent;
				if(parent != null) parent.Add(this);
			}

			public COOPClass NodeClass => @class;

			public List<Node> Children => children;

			public Node ParentNode => parent;

			public int Add(object value) {
				return ((System.Collections.IList) children).Add(value);
			}

			public bool Contains(object value) {
				return ((System.Collections.IList) children).Contains(value);
			}

			public Node find(COOPClass coopClass) {
				if (@class.Equals(coopClass)) return this;
			
				foreach (Node child in children) {
					Node f = child.find(coopClass);
					if (f != null) return f;
				}

				return null;
			}
			
			public Node find(string coopClass) {
				if (@class.ToString().Equals(coopClass)) return this;
			
				foreach (Node child in children) {
					Node f = child.find(coopClass);
					if (f != null) return f;
				}

				return null;
			}

			public void print(int indent) {
				for (int i = 0; i < indent; i++) {
					Console.Write("\t");
				}
				Console.WriteLine(@class.ToString());
				foreach (Node child in children) {
					child.print(indent + 1);
				}
			}

			protected bool Equals(Node other) {
				return Equals(@class, other.@class);
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((Node) obj);
			}

			public override int GetHashCode() {
				return (@class != null ? @class.GetHashCode() : 0);
			}
		}

		private Node head;

		public ClassHierarchy() {
			head = new Node(COOPClass.Base, null);
			addClass(COOPClass.String);
			addClass(COOPClass.Void);
			addClass(COOPPrimitives.@byte);
			addClass(COOPPrimitives.@short);
			addClass(COOPPrimitives.integer);
			addClass(COOPPrimitives.@long);
			addClass(COOPPrimitives.ubyte);
			addClass(COOPPrimitives.@ushort);
			addClass(COOPPrimitives.uinteger);
			addClass(COOPPrimitives.@ulong);
			
			addClass(COOPPrimitives.@float);
			addClass(COOPPrimitives.@double);
		}

		public bool addClass(COOPClass coopClass) {
			Node t;
			addClassPriv(coopClass, out t);
			return t != null;
		}
		
		private bool addClassPriv(COOPClass coopClass, out Node createdNode) {
			Node parent = null;
			if (contains(coopClass)) {
				createdNode = this[coopClass];
				return false;
			}

			if (coopClass.Parent != null) {
				if (!contains(coopClass.Parent)) {
					addClassPriv(coopClass.Parent, out parent);
				} else {
					parent = this[coopClass.Parent];
				}
			}

			Node newNode = new Node(coopClass, parent);
			createdNode = newNode;
			return true;
		}
		
		
		public bool contains(COOPClass coopClass) {
			return contains(head, coopClass);
		}

		private bool contains(Node n, COOPClass coopClass) {
			if (n.NodeClass.Equals(coopClass)) return true;
			foreach (Node nChild in n.Children) {
				if (contains(nChild, coopClass)) return true;
			}

			return false;
		}

		public List<COOPClass> getLineage(COOPClass coopClass) {
			if(getParent(coopClass) == null) return new List<COOPClass>{coopClass};
			List<COOPClass> output = new List<COOPClass>(getLineage(coopClass.Parent));
			output.Add(coopClass);
			return output;
		}

		private Node this[string className] => head.find(className);
		private Node this[COOPClass className] => head.find(className);

		public COOPClass getParent(COOPClass coopClass) {
			return this[coopClass.Parent]?.NodeClass;
		}

		public COOPClass getClass(string className) => this[className].NodeClass;

		public void print() => head.print(0);
	}
}