using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace COOP.core.compiler {
	public class ReservedWords {
		private static readonly string[] reservedWords;

		static ReservedWords() {
			reservedWords = new[] {
				"return",
				"this",
				"if",
				"while",
				"for",
				"do",
				"class"
			};	
		}

		public static bool isReserved(string s) {
			return reservedWords.Contains(s);
		}

		public static string[] getReservedWords() {
			return reservedWords;
		}
	}
}