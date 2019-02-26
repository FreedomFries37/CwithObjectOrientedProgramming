namespace COOP.core.compiler.converters.ConvertedInformation {
	public class FunctionCallConvertedInformation : ConvertedInformation{

		public string ConvertedFunctionCall { get; }

		public FunctionCallConvertedInformation(string convertedFunctionCall) {
			ConvertedFunctionCall = convertedFunctionCall;
		}
	}
}