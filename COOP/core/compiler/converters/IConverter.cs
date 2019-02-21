using System.Collections.ObjectModel;
using COOP.core.inheritence;
using Microsoft.VisualBasic.CompilerServices;

namespace COOP.core.compiler.converters {
	public interface IConverter<T, R> where R : ConvertedInformation {
		
		Collection<R> convert(T coopObject, ClassHierarchy hierarchy);
	}
}