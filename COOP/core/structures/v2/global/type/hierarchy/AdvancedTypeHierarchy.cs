using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using COOP.core.structures.v2.global.type.included;

namespace COOP.core.structures.v2.global.type.hierarchy {
	public class AdvancedTypeHierarchy {

		protected abstract class Node {
			
			public Node<COOPAbstract> parent { get; }
			public List<Node<COOPInterface>> interfaces { get; }

			public Node(Node<COOPAbstract> parent, List<Node<COOPInterface>> interfaces) {
				this.parent = parent;
				this.interfaces = interfaces;
			}

			public abstract bool isClass();
			public abstract bool isAbstract();
			public abstract bool isInterface();

			public static Node<COOPInterface> createInterfaceNode(COOPInterface @interface) {
				return new Node<COOPInterface>(@interface, null, new List<Node<COOPInterface>>());
			}

			public static Node<COOPAbstract> createAbstractNode(COOPAbstract @abstract, Node<COOPAbstract> parent) {
				return new Node<COOPAbstract>(@abstract, parent, new List<Node<COOPInterface>>());
			}
		}
		protected class Node<T> : Node where T : COOPType {
			public T type { get; }
			

			public override bool isClass() => type is COOPClass;
			public override bool isAbstract() => type is COOPAbstract && !isClass();

			public override bool isInterface() => !isAbstract() && isClass();

			public Node(T type, Node<COOPAbstract> parent, List<Node<COOPInterface>> interfaces) : base(parent, interfaces) {
				this.type = type;
			}
		}




		private Dictionary<COOPInterface, Node<COOPInterface>> interfaceNodes;
		private Dictionary<COOPAbstract, Node<COOPAbstract>> abstractNodes;
		private Node<COOPAbstract> head;

		public AdvancedTypeHierarchy() {
			interfaceNodes = new Dictionary<COOPInterface, Node<COOPInterface>>();
			abstractNodes = new Dictionary<COOPAbstract, Node<COOPAbstract>>();
			head = Node.createAbstractNode(IncludedClasses.Object, null);
		}


		public bool add(COOPType type) {
			if (type is COOPAbstract) {
				return add(type as COOPAbstract);
			}

			return add(type as COOPInterface);
		}

		public bool add(COOPInterface @interface) {
			if (contains(@interface)) return false;

			Node<COOPInterface> interfaceNode = Node.createInterfaceNode(@interface);
			foreach (COOPInterface parentInterface in @interface.parentInterfaces) {
				if (!contains(parentInterface)) add(parentInterface);
				interfaceNode.interfaces.Add(interfaceNodes[parentInterface]);
			}

			interfaceNodes.Add(@interface, interfaceNode);
			return true;
		}

		public bool add(COOPAbstract @abstract) {
			if (@abstract == null) return false;
			if (contains(@abstract)) return false;
			if (!contains(@abstract.parent)) add(@abstract.parent);

			Node<COOPAbstract> parent = @abstract.parent != null ? abstractNodes[@abstract.parent] : null;

			Node<COOPAbstract> created = Node.createAbstractNode(@abstract, parent);
			
			foreach (COOPInterface parentInterface in @abstract.parentInterfaces) {
				if (!contains(parentInterface)) add(parentInterface);
				created.interfaces.Add(interfaceNodes[parentInterface]);
			}

			abstractNodes.Add(@abstract, created);
			return true;
		}
		

		public bool contains(COOPAbstract @abstract) {
			if (@abstract == null) return false;
			return abstractNodes.ContainsKey(@abstract);
		}

		public bool contains(COOPInterface @interface) {
			if (@interface == null) return false;
			return interfaceNodes.ContainsKey(@interface);
		}

		public bool contains(COOPType type) {
			return contains(type as COOPAbstract) || contains(type as COOPInterface);
		}
		
		

		public COOPClass findNextAvailableParentClass(COOPAbstract a) {
			Node<COOPAbstract> parent = abstractNodes[a.parent];
			if (parent == null) return null;
			if(parent.isClass()) return parent.type as COOPClass;
			return findNextAvailableParentClass(a.parent);
		}

		public CApplicableStructHierarchy convert() {
			CApplicableStructHierarchy hierarchy = new CApplicableStructHierarchy(IncludedClasses.Object);

			Collection<COOPAbstract> vistedClasses = new Collection<COOPAbstract>();
			Queue<Node> queue = new Queue<Node>();
			queue.Enqueue(head);
			while (queue.Count >= 0) {
				Node current = queue.Dequeue();
				

				if (current.isClass()) {
					Node<COOPClass> c = current as Node<COOPClass>;
					
					if (c == null) return null;
					vistedClasses.Add(c.type);
					hierarchy.add(c.type, findNextAvailableParentClass(c.type));
				}else if (current.isAbstract()) {
					Node<COOPAbstract> node = current as Node<COOPAbstract>;
					if (node == null) return null;
					vistedClasses.Add(node.type);
				}
				
				
				List<Node> nextNodes =  new List<Node>(
					from f in abstractNodes.Values where vistedClasses.Contains(f.parent.type) select f
				);
				nextNodes.ForEach(queue.Enqueue);
			}
			
			return hierarchy;
		}
		
	}
}