namespace FileCondenser.core {
	public interface ICondenser {
		(string, HuffmanChain) Condense(string w);
	}
}