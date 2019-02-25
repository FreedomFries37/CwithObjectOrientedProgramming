namespace COOP.core.compiler {
	public class FunctionCallConvertedInformation : ConvertedInformation{

		public string ConvertedFunctionCall { get; }

		public FunctionCallConvertedInformation(string convertedFunctionCall) {
			ConvertedFunctionCall = convertedFunctionCall;
		}
	}
}