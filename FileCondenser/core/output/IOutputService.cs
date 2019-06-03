namespace FileCondenser.core.output {
	public interface IOutputService<T> {
		string CreateOutput(T tObject);
	}
}