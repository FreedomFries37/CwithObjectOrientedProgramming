using System.Collections.Generic;
using System.Linq;
using FileCondenser.core;
using NUnit.Framework;

namespace COOPTests.CondenserTests {
	[TestFixture]
	public class HeapTests {
		private static void AssertHeapSort(Heap<int> heap, IEnumerable<int> expected) {
			var sorted = new List<int>();
			while (heap.count > 0) sorted.Add(heap.Pop());
			Assert.IsTrue(sorted.SequenceEqual(expected));
		}

		[Test]
		public void TestHeapBySorting() {
			var minheap = new Heap<int>(Comparer<int>.Create((a, b) => b - a)) {9, 8, 4, 1, 6, 2, 7, 4, 1, 2};
			AssertHeapSort(minheap, minheap.OrderBy(i => i).ToArray());

			var maxheap = new Heap<int> {9, 8, 4, 1, 6, 2, 7, 4, 1, 2};
			AssertHeapSort(minheap, minheap.OrderBy(i => -i).ToArray());
		}
	}
}