namespace StateMachine.AsyncEx {

	public interface IAsyncOperation<T> {
		Task<T> GetResult();
	}

}