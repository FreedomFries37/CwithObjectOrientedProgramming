namespace FileCondenser.core.input {
	public interface IInputService<T> {
		T CreateFromInput(string w);
	}
}