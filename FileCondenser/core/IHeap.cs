using System;
using System.Collections.Generic;

namespace FileCondenser.core {
	public interface IHeap<T> : IEnumerable<T>{
		
		T Peak();
		
		void Add(T o);
		
		T Pop();

		bool IsEmpty();

	}
}