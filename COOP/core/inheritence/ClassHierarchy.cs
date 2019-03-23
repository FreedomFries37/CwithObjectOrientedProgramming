using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using COOP.core.compiler;
using COOP.core.compiler.converters;
using COOP.core.compiler.converters.ConvertedInformation;
using COOP.core.structures;
using COOP.core.structures.v1;

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
			
			public List<Node> linearization() {
				var output = new List<Node> {this};

				foreach (Node child in children) {
					output.AddRange(child.linearization());
				}

				return output;
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

		public List<COOPClass> linearization() {
			return (from f in head.linearization() select f.NodeClass).ToList();

		}

		private Node this[string className] => head.find(className);
		private Node this[COOPClass className] => head.find(className);

		public COOPClass getParent(COOPClass coopClass) {
			return this[coopClass.Parent]?.NodeClass;
		}

		public COOPClass getClass(string className) => this[className].NodeClass;

		public void print() => head.print(0);

		public void createAllCFiles() {
			createAllCFiles("");
		}

		public void createAllCFiles(string directory) {
			var f = linearization();
			COOPClassConverter c = new COOPClassConverter();
			string mainMethod = "";
			string mainHeader = "";
			foreach (COOPClass coopClass in from h in f where h.genFile select h) {


				Collection<FileConvertedInformation> fileConvertedInformations = c.convert(coopClass, this);
				foreach (FileConvertedInformation fileConvertedInformation in fileConvertedInformations) {

					if (fileConvertedInformation.hasMainMethod) {
						mainHeader = fileConvertedInformation.IntendedFileName;
						mainMethod = fileConvertedInformation.mainMethod;
					}

					StreamWriter w = File.CreateText(directory + fileConvertedInformation.IntendedFileName);
					w.Write(fileConvertedInformation.FileContents);
					w.Flush();
					w.Close();
				}
			}

			if (mainHeader != null & mainMethod != null) {
				mainHeader += '"';
				mainHeader = "\"" + mainHeader;

				string string_location = "String_protected.h";

				StreamWriter m = File.CreateText(directory + "main.c");
				m.Write($@"
#include {mainHeader}
#include {string_location}

int main(int argv, char* argc){{
	struct String* strings = (struct String*) malloc(sizeof(struct String)*argv);
	for(int i = 0; i < argv; i++){{
		struct String s = {{.ptr = arc[i]}};
		strings[i] = s;
	}}
	return {mainMethod}(strings);
}}
			");
				m.Flush();
				m.Close();
			}

		}
	}
}