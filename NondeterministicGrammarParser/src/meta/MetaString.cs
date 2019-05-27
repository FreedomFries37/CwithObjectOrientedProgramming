namespace NondeterministicGrammarParser.meta {
	
	/// <summary>
	/// String that holds metadata
	/// </summary>
	public class MetaString {
		
		public string Value { get; }
		public int LineNumber { get; }
		public int ColumnNumber { get; }

		public MetaString(string value, int lineNumber = 0, int columnNumber = 0) {
			Value = value;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
		}

		public override string ToString() {
			return Value;
		}
		
		public static MetaString operator+(MetaString a, MetaString b) {
			return new MetaString(a.Value + b.Value, a.LineNumber, a.ColumnNumber);
		}
		
		public static MetaString operator+(MetaString a, string b) {
			return new MetaString(a.Value + b, a.LineNumber, a.ColumnNumber);
		}
	}
}