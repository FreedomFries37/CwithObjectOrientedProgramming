using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FileCondenser.core {
	
	
	public class Heap<T> : IHeap<T> {

		private const int INITIAL_SIZE = 1;
		private const int GROW_RATE = 2;
		private const int MIN_GROW = 1;
		
		public int count { get; private set; }

		public int capacity { get; private set; } = INITIAL_SIZE;
		private T[] _heap = new T[INITIAL_SIZE];
		protected IComparer<T> comparer { get;  }

		

		public Heap(IComparer<T> comparer) {
			this.comparer = comparer;
		}

		public Heap() : this(Comparer<T>.Default){ }

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator() {
			return _heap.Take(count).GetEnumerator();
		}

		public T Peak() {
			if(count == 0) throw new InvalidOperationException("Heap Empty");
			return _heap[0];
		}

		public void Add(T o) {
			if (count == capacity) {
				Grow();
			}

			_heap[count++] = o;
			BubbleUp(count - 1);
		}

		public T Pop() {
			if(count == 0) throw new InvalidOperationException("Heap Empty");
			T output = _heap[0];
			count--;
			
			Swap(count, 0);
			BubbleDown(0);

			return output;
		}

		private void BubbleUp(int i) {
			if (i == 0 || Dominates(_heap[Parent(i)], _heap[i])) return;
			
			Swap(i, Parent(i));
			BubbleUp(Parent(i));
		}

		private void BubbleDown(int i) {
			int dominating = Dominating(i);
			if(dominating == i) return;
			Swap(i, dominating);
			BubbleDown(dominating);
		}

		private int Dominating(int i) {
			int node = i;
			node = GetDominating(LeftChild(i), node);
			node = GetDominating(RightChild(i), node);
			return node;
		}

		private int GetDominating(int a, int b) {
			if (a < count &&
				!Dominates(_heap[b], _heap[a]))
				return a;
			return b;
		}

		protected virtual bool Dominates(T a, T b) {
			return comparer.Compare(a, b) >= 0;
		}

		private static int Parent(int i) {
			return (i + 1) / 2 - 1;
		}

		private static int LeftChild(int i) {
			return (i + 1) * 2 - 1;
		}
		
		private static int RightChild(int i) {
			return (i + 1) * 2;
		}

		private void Grow() {
			int newCapacity = capacity * GROW_RATE + MIN_GROW;
			var newHeap = new T[newCapacity];

			Array.Copy(_heap, newHeap, capacity);
			_heap = newHeap;
			capacity = newCapacity;
		}

		private void Swap(int a, int b) {
			T temp = _heap[a];
			_heap[a] = _heap[b];
			_heap[b] = temp;
		}

		public override string ToString() {
			return "[" + string.Join(", ", this) + "]";
		}

		public bool IsEmpty() {
			return count == 0;
		}
	}
}